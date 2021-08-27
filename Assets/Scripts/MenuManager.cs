using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MenuManager : MonoBehaviour
{
    Menu currentMenu;
    public static MenuManager instance;
    public TerminalData data;
    public TerminalRenderer menuRenderer;
    Dictionary<string, Menu> menus;
    List<string> menuStack;

    bool initMenus = true;

    public void generateMenus()
    {

        //  *   MENUS   *   //
        menus = new Dictionary<string, Menu>();
        menuStack = new List<string>();
        menuStack.Add("main");


        //  **  MAIN    **  //
        Translation tr = data.translationData.translation;

        Menu main = new Menu(menuRenderer);
        menus.Add("main", main);

        main.addText(tr.ui.main.terminalWelcome);
        main.addSpace();
        main.addText(tr.ui.main.selectEntries);
        main.addSeparator("-");
        main.addSpace();
        main.addButton("1) " + tr.ui.main.emails_btn, () => MenuManager.instance.switchMenu("mail"));
        main.addButton("2) " + tr.ui.main.logs_btn, () => MenuManager.instance.switchMenu("logs"));
        main.addButton("0) " + tr.ui.main.exit_btn, () => MenuManager.instance.exit());


        if (initMenus) // only first time calling function
        {
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

            initMenus = false;
        }

        //  **  MAIL    **  //
        Menu mail = new Menu(menuRenderer);
        menus.Add("mail", mail);

        mail.addText(tr.ui.mail.header);
        mail.addSeparator("-");
        mail.addText(string.Format(tr.ui.mail.mailCounter, 0));
        mail.addSpace();
        mail.addButton("0) " + tr.ui.button_back, () => MenuManager.instance.back());

        //  **  Logs    **  //
        Menu logs = new Menu(menuRenderer);
        menus.Add("logs", logs);

        // loading logs itselves        

        logs.addText(tr.ui.logs.header);
        logs.addSeparator("-");

        // creating log menus
        int iter = 0;
        foreach (TerminalData.Document doc in MenuManager.instance.data.documents)
        {
            string key = "log-" + iter.ToString();
            menus.Add(key, new Menu(menuRenderer));
            Menu log = menus[key];
            log.addText(doc.value);
            log.addSeparator("-");
            log.addButton("0) " + tr.ui.button_back, () => MenuManager.instance.back());

            // add link to logs menu
            Debug.Log(key);
            logs.addButton((iter + 1).ToString() + ") " + doc.displayName, () => MenuManager.instance.switchMenu(key));

            iter++;
        }
        //

        logs.addSpace();
        logs.addButton("0) " + tr.ui.button_back, () => MenuManager.instance.back());
    }
    public void Awake()
    {
        //making manager singleton
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        //  reloading entries
        MenuManager.instance.data.updateEntries();
        generateMenus();
    }

    public void Update()
    {
        currentMenu = menus["main"];
        if (menuStack.Count > 0)
            currentMenu = menus[menuStack[menuStack.Count - 1]];
        currentMenu.Render();

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
    }

    public void switchMenu(string menuName)
    {
        if (menus.ContainsKey(menuName))
            menuStack.Add(menuName);
        else
            Debug.LogError("No menu named \"" + menuName + "\"");
    }

    public void back()
    {
        if (menuStack.Count > 1)
        {
            Debug.Log("Changed menu: " + menuStack[menuStack.Count - 1] + " -> " + menuStack[menuStack.Count - 2]);
            menuStack.RemoveAt(menuStack.Count - 1);
        }
    }

    public void exit()
    {
        menuStack = new List<string>();
        menuStack.Add("main");
        Debug.Log("Exited App");
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

    public void down()
    {
        int offset = 1;
        while (menuItems[(selectedItemPos + offset) % menuItems.Count].type != MI_TYPE.button && offset <= menuItems.Count)
        {
            offset++;
        }
        if (menuItems[(selectedItemPos + offset) % menuItems.Count].type == MI_TYPE.button)
        {
            selectedItemPos = (selectedItemPos + offset) % menuItems.Count;
            selectedItem = menuItems[selectedItemPos];
        }
    }

    static int mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }
    public void up()
    {
        int offset = 1;
        while (menuItems[mod(selectedItemPos - offset, menuItems.Count)].type != MI_TYPE.button && offset <= menuItems.Count)
        {
            offset++;
        }
        if (menuItems[mod(selectedItemPos - offset, menuItems.Count)].type == MI_TYPE.button)
        {
            selectedItemPos = mod(selectedItemPos - offset, menuItems.Count);
            selectedItem = menuItems[selectedItemPos];
        }
    }

    public void submit()
    {
        if (selectedItem != null)
        {
            if (selectedItem.type == MI_TYPE.button)
                ((MenuButton)(selectedItem)).onClick();
        }
        else
        {
            MenuManager.instance.back();
        }
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
            if (menuItems[i].type == MI_TYPE.button)
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

// TODO : Image viewer
/*public class EntryViewer : Menu
{
    public EntryViewer(TerminalRenderer renderer, string entry) : base(renderer){}
}*/

public enum MI_TYPE { separator, text, button, image };

public abstract class MenuItem
{
    public MI_TYPE type;
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
public class MenuButton : MenuItem
{
    public MenuButton(int id, string text, Action func) : base(id, text) { type = MI_TYPE.button; onClick += func; }
    public Action onClick;
}
public class MenuText : MenuItem
{
    public MenuText(int id, string text) : base(id, text) { type = MI_TYPE.text; }
}

public class MenuSeparator : MenuItem
{
    public MenuSeparator(int id, string text) : base(id, text) { type = MI_TYPE.separator; }
}