using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class TerminalRenderer : MonoBehaviour
{
    public TextMeshProUGUI tmesh;
    public TerminalData data;
    // Start is called before the first frame update
    public char[,] screen;
    string header;
    int line = 0;

    List<List<WrappingTags>> tags;
    void Start()
    {
        header = data.header;
        tmesh.text = "";
        screen = new char[data.MaxLineCount, data.MaxLineLength];
        tags = new List<List<WrappingTags>>();

        Clear();
        RedrawBorder();
        RedrawScreen();
    }

    public void RedrawBorder()
    {
        for(int j = 0; j < Math.Max(data.MaxLineCount, data.MaxLineLength); j++)
        {
            if(j < data.MaxLineCount)
            {
                screen[j,0] = screen[j,data.MaxLineLength-1] = '│';
            }

            if(j < data.MaxLineLength)
            {
                screen[0,j] = screen[data.MaxLineCount-1,j] = '─';
            }

            screen[0,0] = '┌';
            screen[0,data.MaxLineLength-1] = '┐';
            screen[data.MaxLineCount-1,0] = '└';
            screen[data.MaxLineCount-1,data.MaxLineLength-1] = '┘';
        }


        for(int j = 0; j < Math.Min(header.Length, data.MaxLineLength-2); j++)
        {
            screen[0,j+1] = header[j];
        }

        int mx = Math.Min(data.version.Length, data.MaxLineLength-2);
        for(int j = 0; j < mx; j++)
        {
            screen[data.MaxLineCount - 1, data.MaxLineLength - 2 - j] = data.version[mx - j - 1];
        }
    }

    public class WrappingTags
    {
        public int from;
        public int to;
        public string open;
        public string closing;
        public WrappingTags(int from, int to)
        {
            this.from = from;
            this.to = to;
            this.open = "";
            this.closing = "";
        }
        public WrappingTags(int from, int to, string open, string closing)
        {
            this.from = from;
            this.to = to;
            this.open = open;
            this.closing = closing;
        }
        
        public void addTag(string name)
        {
            open = "<" + name + ">" + open;
            closing = closing + "</" + name + ">";
        }
        public void addTag(string name, string param)
        {
            open = "<" + name + "=" + param + ">" + open;
            closing = closing + "</" + name + ">";
        }
    }

    public void AddEntry(string text, bool isSelected)
    {
        text = text.Replace('\r',' ');
        string leftover = "";
        if(text.Length > data.maxLength)
        {
            leftover += text.Substring(data.maxLength, text.Length - data.maxLength);
            text = text.Substring(0, data.maxLength);
        }

        int tmp = text.IndexOf('\n');
        if(tmp >= 0)
        {
            leftover = text.Substring(tmp+1, text.Length-tmp-1) + leftover;
            text = text.Substring(0, tmp);
        }

        for(int i = 0; i < data.maxLength; i++)
        {
            if(i < text.Length)
                screen[line + data.PaddingV, data.PaddingH + i] = text[i];
        }
        while(tags.Count <= line + data.PaddingV)
            tags.Add(new List<WrappingTags>());
        Color col = data.defaultColor;
        if(isSelected)
        {
            col = data.activeColor;
            tags[line + data.PaddingV].Add(new WrappingTags(data.PaddingH, data.maxLength - 1, "<mark=#" + toHex(data.activeBGColor) + ">","</mark>"));
        }
        tags[line + data.PaddingV].Add(new WrappingTags(data.PaddingH, data.maxLength - 1, "<color=#" + toHex(col) + ">","</color>"));

        line++;
        //dealing with left substring
        if(leftover.Length > 0)
            AddEntry(leftover, isSelected);
    }

    public void Clear()
    {
        line = 0;
        screen = new char[data.MaxLineCount, data.MaxLineLength];
        for(int i = 0; i < data.MaxLineCount; i++)
        {
            for(int j = 0; j < data.MaxLineLength; j++)
            {
                screen[i,j] = data.noBreak;
            }
        }
        tags = new List<List<WrappingTags>>();
    }
    public void RedrawScreen()
    {
        string text = "";
        for(int i = 0; i < data.MaxLineCount; i++)
        {
            for(int j = 0; j < data.MaxLineLength; j++)
            {
                string openTags = "";
                string closingTags = "";
                if(tags.Count > i)
                {
                    foreach(WrappingTags tag in tags[i])
                    {
                        if(tag.from == j)
                            openTags = tag.open + openTags;
                        if(tag.to == j)
                            closingTags = closingTags + tag.closing;
                    }
                }
                text += openTags;
                if(screen[i,j] == 0 || screen[i,j] == ' ' || screen[i,j] == '\n')
                {
                    text += data.noBreak;
                }
                else
                {
                    text += screen[i,j];
                }
                text += closingTags;
            }
            text += data.newLine;
        }

        tmesh.text = text;
    }

    public string toHex(Color c)
    {
        if(c.a < 1)
            return ((int)(c.r * 255)).ToString("X2") + ((int)(c.g * 255)).ToString("X2") + ((int)(c.b * 255)).ToString("X2") + ((int)(c.a * 255)).ToString("X2");
        else
            return ((int)(c.r * 255)).ToString("X2") + ((int)(c.g * 255)).ToString("X2") + ((int)(c.b * 255)).ToString("X2");
    }
}
