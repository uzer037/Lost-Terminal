using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(TerminalRenderer))]
public class MenuManager : MonoBehaviour
{
    Func<string, string> getPhrase = (string value) => Lean.Localization.LeanLocalization.GetTranslationText("Phrase_" + value, fallback: "ERR-NOT_FOUND");

    Menu currentMenu;
    public static MenuManager instance;
    public TerminalData data;
    TerminalRenderer menuRenderer;
    Dictionary<string, Menu> menus;
    List<string> menuStack;

    List<Entry> entries;

    bool initMenus = true;
    int incorrectPasswordInput = 0;
    int incorrectPasswordInputLimit = 3;


    /*==========================================*
    *                                          *
    *      START OF MENU LAYOUT SECTION        *
    *                                          *
    * =========================================*/
    public void generateMenus()
    {
        //  *   MENUS   *   //
        menus = new Dictionary<string, Menu>();
        menuStack = new List<string>();
        menuStack.Add("main");


        //  **  MAIN    **  //

        Menu main = new Menu(menuRenderer);
        menus.Add("main", main);

        main.addText(getPhrase("main.terminalWelcome"));
        main.addSeparator("-");
        main.addSpace();
        main.addText(getPhrase("main.selectEntries"));
        main.addSeparator("-");
        main.addSpace();
        main.addButton("1) " + getPhrase("main.emails_btn") + ".", () => MenuManager.instance.switchMenu("mail"));
        main.addButton("2) " + getPhrase("main.logs_btn") + ".", () => MenuManager.instance.switchMenu("logs"));
        main.addButton("0) " + getPhrase("main.exit_btn") + ".", () => MenuManager.instance.exit());


        if (initMenus) // only first time calling function
        {
            /*
            //  **  Language selection   ** //
            Menu langSelect = new Menu(menuRenderer);
            menus.Add("langSelect", langSelect);
            menuStack.Add("langSelect");
            langSelect.addText("Please select system language:");
            langSelect.addSpace();

            int n = 0;
            foreach (Translation transl in MenuManager.instance.data.translationData.translations)
            {
                n++;
                langSelect.addButton(n.ToString() + ") " + transl.editorData.languageFullName, () =>
                {
                    MenuManager.instance.data.setLanguage(transl.language);
                    tr = data.translationData.translation;
                    generateMenus();
                });
            }
            */

            //  **  Login   **  //
            Menu loginMenu = new Menu(menuRenderer);
            menus.Add("loginMenu", loginMenu);
            menuStack.Add("loginMenu");

            // building loginMenu
            void rebuildLoginMenu()
            {
                loginMenu.clearLayout();
                loginMenu.addSpace();
                loginMenu.addText(getPhrase("login.title"));
                switch(incorrectPasswordInput)
                {
                    case 0:
                        {
                            loginMenu.addSpace();
                            break;
                        }
                    default:
                        {
                            loginMenu.addText(string.Format(getPhrase("login.incorrectPass"), incorrectPasswordInputLimit - incorrectPasswordInput));
                            break;
                        }
                }
                loginMenu.addSpace();
                loginMenu.addText(getPhrase("login.prompt"));
                loginMenu.addSpace();
                loginMenu.addText(getPhrase("login.username"));
                string correctPassword = "projectmadmen";
                //set max password length as 16 cuz it's looking better
                loginMenu.addPasswordField(16, correctPassword, () =>
                    {
                        Debug.Log("Password is correct");
                        MenuManager.instance.menustack_pop();
                    },
                    () =>
                    {
                        Debug.Log("Password is wrong");
                        incorrectPasswordInput++;
                        rebuildLoginMenu();
                    });
                RedrawTopMenu();
            }

            rebuildLoginMenu();

            initMenus = false;
        }

        //  **  MAIL    **  //
        Menu mail = new Menu(menuRenderer);
        menus.Add("mail", mail);

        mail.addText(getPhrase("mail.header"));
        mail.addSeparator("-");
        mail.addText(string.Format(getPhrase("mail.mailCounter"), 0));
        mail.addSpace();
        mail.addButton("0) " + getPhrase("button_back"), () => MenuManager.instance.menustack_pop());

        //  **  Logs    **  //
        Menu logs = new Menu(menuRenderer);
        menus.Add("logs", logs);

        // loading logs themselves        
        entries = new List<Entry>();
        foreach(var phrase in Lean.Localization.LeanLocalization.Instances[0].transform.GetComponentsInChildren<Lean.Localization.LeanPhrase>())
        {
            Lean.Localization.LeanPhrase.Entry entry = new Lean.Localization.LeanPhrase.Entry();
            phrase.TryFindTranslation(Lean.Localization.LeanLocalization.Instances[0].CurrentLanguage, entry: ref entry);
            if(entry != null && entry.Object != null)
                entries.Add(entry.Object as Entry);
        }

        // creating log menus
        logs.addText(getPhrase("logs.header"));
        logs.addSeparator("-");

        int iter = 0;
        foreach (Entry ent in entries)
        {
            string key = "log-" + iter.ToString();
            menus.Add(key, new Menu(menuRenderer));
            Menu log = menus[key];
            log.addText(ent.value);
            log.addSeparator("-");
            log.addButton("0) " + getPhrase("button_back"), () => MenuManager.instance.menustack_pop());

            // add link to logs menu
            Debug.Log(key);
            logs.addButton((iter + 1).ToString() + ") " + ent.title, () => MenuManager.instance.switchMenu(key));

            iter++;
        }
        
        logs.addSpace();
        logs.addButton("0) " + getPhrase("button_back"), () => MenuManager.instance.menustack_pop());

        currentMenu = menus[menuStack[menuStack.Count - 1]];
    }

    /*==========================================*
     *                                          *
     *      END OF MENU LAYOUT SECTION          *
     *                                          *
     * =========================================*/
    public void Awake()
    {
        //making manager singleton
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        menuRenderer = gameObject.GetComponent<TerminalRenderer>();
    }

    public void Start()
    {
        generateMenus();
        currentMenu.Render();
    }

    public void Update()
    {
        currentMenu = menus[menuStack[menuStack.Count - 1]];

        if (Input.GetButtonDown("Up"))
        {
            currentMenu.up();
        }

        if (Input.GetButtonDown("Down"))
        {
            currentMenu.down();
        }

        if (Input.GetButtonDown("Submit"))
        {
            currentMenu.submit();
        }

        if(Input.inputString != "")
        {
            if(currentMenu.isOverInputField())
            {
                currentMenu.input(Input.inputString);
            }
        }
    }

    public void switchMenu(string menuName)
    {
        if (menus.ContainsKey(menuName))
            menuStack.Add(menuName);
        else
            Debug.LogError("No menu named \"" + menuName + "\"");
    }

    public void menustack_pop()
    {
        if (menuStack.Count > 1)
        {
            Debug.Log("Changed menu: " + menuStack[menuStack.Count - 1] + " -> " + menuStack[menuStack.Count - 2]);
            menuStack.RemoveAt(menuStack.Count - 1);
            currentMenu = menus[menuStack[menuStack.Count - 1]];
            RedrawTopMenu();
        }
    }

    public void RedrawTopMenu()
    {
        menus[menuStack[menuStack.Count - 1]].Render();
    }

    public void exit()
    {
        menuStack = new List<string>();
        menuStack.Add("main");
        Debug.Log("Exited App");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}

public class Menu
{
    int id = 0;
    int getNewId
    {
        get { return id++; }
        set { id = value; }
    }
    MenuItem selectedItem;
    int selectedItemPos;
    TerminalRenderer renderer;
    List<MenuItem> menuItems;
    public Menu(TerminalRenderer renderer)
    {
        this.renderer = renderer;
        menuItems = new List<MenuItem>();
        id = 0;
        selectedItem = null;
        selectedItemPos = -1;
    }

    public bool isOverInputField()
    {
        return selectedItem is MenuInputField;
    }

    public void input(string input)
    {
        MenuInputField field = selectedItem as MenuInputField;
        if(field != null)
        {
            string str = field.unformattedText + input;
            int pos = str.IndexOf('\b');
            while( pos != -1 )
            {
                if(pos > 0)
                    str = str.Remove(pos - 1, 2);
                else
                    str = str.Remove(pos, 1);
                pos = str.IndexOf('\b');
            }
            field.unformattedText = str.Substring(0,Mathf.Min(str.Length, field.length));
            if(field.isObfuscated)
                field.text = new string(field.obfuscationSymbol, field.unformattedText.Length);
            else
                field.text = field.unformattedText;
            if (field.text.Length < field.length)
                field.text += new string('_', field.length - field.text.Length);
        }
        updateMenu();
    }

    void updateMenu()
    {
        MenuManager.instance.RedrawTopMenu();
    }

    public void down()
    {
        int offset = 1;
        while( !(menuItems[(selectedItemPos + offset) % menuItems.Count] is MenuInteractibleItem) && offset <= menuItems.Count)
        {
            Debug.Log(offset);
            offset++;
        }
        if (menuItems[(selectedItemPos + offset) % menuItems.Count] is MenuInteractibleItem)
        {
            selectedItemPos = (selectedItemPos + offset) % menuItems.Count;
            selectedItem = menuItems[selectedItemPos];
        }
        updateMenu();
    }

    static int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }
    public void up()
    {
        int offset = 1;
        while( !(menuItems[mod(selectedItemPos - offset, menuItems.Count)] is MenuInteractibleItem) && offset <= menuItems.Count)
        {
            offset++;
        }

        if (menuItems[mod(selectedItemPos - offset, menuItems.Count)] is MenuInteractibleItem)
        {
            selectedItemPos = mod(selectedItemPos - offset, menuItems.Count);
            selectedItem = menuItems[selectedItemPos];
        }
        updateMenu();
    }

    public void submit()
    {
        if (selectedItem != null)
        {
            if (selectedItem is MenuInteractibleItem)
                ((MenuInteractibleItem)(selectedItem)).onClick();
        }
        else
        {
            MenuManager.instance.menustack_pop();
        }
        updateMenu();
    }

    public void clearLayout()
    {
        id = 0;
        selectedItem = null;
        selectedItemPos = -1;
        menuItems.Clear();
    }
    public void addText(string text)
    {
        text = text.Replace("<BR>", "\n");
        text = text.Replace("<Br>", "\n");
        text = text.Replace("<br>", "\n");
        menuItems.Add(new MenuText(getNewId, text));
        id++;
    }
    public void addButton(string text, Action func)
    {
        menuItems.Add(new MenuButton(id, text, func));
        id++;

        for (int i = 0; selectedItem == null && i < menuItems.Count; i++)
        {
            if (menuItems[i] is MenuButton)
            {
                selectedItem = menuItems[i];
                selectedItemPos = i;
            }
        }
    }
    public void addPasswordField(int length, string correctPassword, Action funcOnValid, Action funcOnInvalid, char obfuscationSymbol = '*')
    {
        MenuPasswordField field = new MenuPasswordField(id, length, correctPassword, funcOnValid, funcOnInvalid, obfuscationSymbol: obfuscationSymbol);
        if (field.isObfuscated)
            field.text = new string(field.obfuscationSymbol, field.unformattedText.Length);
        else
            field.text = field.unformattedText;
        if (field.text.Length < field.length)
            field.text += new string('_', field.length - field.text.Length);
        menuItems.Add(field);

        id++;

        for (int i = 0; selectedItem == null && i < menuItems.Count; i++)
        {
            if (menuItems[i] is MenuInputField)
            {
                selectedItem = menuItems[i];
                selectedItemPos = i;
            }
        }
    }

    public void addSeparator(string separator)
    {
        string str = "";
        for (int i = 0; i < MenuManager.instance.data.maxLength; i++)
        {
            str += separator;
        }
        menuItems.Add(new MenuSeparator(id, str));
        id++;
    }
    public void addSpace()
    {
        addSeparator(" ");
    }

    public void Render()
    {
        renderer.Clear();
        foreach (var item in menuItems)
        {
            bool isSelected = (selectedItem != null && item == selectedItem);
            renderer.AddEntry(item.text, isSelected);
        }
        renderer.RedrawBorder();
        renderer.RedrawScreen();
    }
}

// ** Menu **

public abstract class MenuItem
{
    public string text;
    public int id;
    public MenuItem(int id, string text)
    {
        this.id = id;
        this.text = text;
    }
    public MenuItem(int id)
    {
        this.id = id;
        this.text = "";
    }
}

public abstract class MenuInteractibleItem : MenuItem
{
    public MenuInteractibleItem(int id, string text, Action func) : base(id, text)
    {
        onClick += func;
    }
    public Action onClick;
}
public class MenuButton : MenuInteractibleItem
{
    public MenuButton(int id, string text, Action func) : base(id, text, func) { }
    
}
public abstract class MenuInputField : MenuInteractibleItem
{
    public int length;
    public string unformattedText;
    public bool isObfuscated;
    public char obfuscationSymbol;
    public MenuInputField(int id, int length, Action func, bool isObfuscated = false, char obfuscationSymbol = '*') : base(id, "", ()=> { })
    {
        this.length = length;
        this.isObfuscated = isObfuscated;
        this.obfuscationSymbol = obfuscationSymbol;
        unformattedText = "";
    }
}

public class MenuPasswordField : MenuInputField
{
    string correctPassword;
    Action onValid;
    Action onInvalid;
    void Validate(string password)
    {
        if(password == correctPassword)
        {
            onValid();
        }
        else
        {
            onInvalid();
        }
    }
    public MenuPasswordField(int id, int length, string correctPassword, Action funcOnValid, Action funcOnInvalid, char obfuscationSymbol = '*') : base(id, length, () => { }, isObfuscated: true, obfuscationSymbol: obfuscationSymbol)
    {
        this.correctPassword = correctPassword;
        onClick += () => Validate(unformattedText);
        onValid += funcOnValid;
        onInvalid += funcOnInvalid;
    }
}
public class MenuText : MenuItem
{
    public MenuText(int id, string text) : base(id, text) { }
}

public class MenuSeparator : MenuItem
{
    public MenuSeparator(int id, string text) : base(id, text) { }
}