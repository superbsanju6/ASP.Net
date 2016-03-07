<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EquationEditor.aspx.cs" Inherits="Thinkgate.Controls.Assessment.ContentEditor.EditorControls.EquationEditor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Equation Editor</title>
   <script type="text/javascript" language="javascript">
       var currEditorText = new Array();
       document.onkeydown = internetExploderBackspaceFix;
       document.onkeypress = captureKeyPress;
       
       function captureKeyPress(e) {
           var code;
           if (!e) var e = window.event;
           if (e.keyCode) code = e.keyCode;
           else if (e.which) code = e.which;
           var character = String.fromCharCode(code);
           if (code >= 33 && code <= 125) {
               addCharacter(character);
           } else if (code == 32) {
               addCharacter('&nbsp;');
           } else if (code == 8) {
               return false;
           }
       }

       function internetExploderBackspaceFix(e) {
           var code;
           if (!e) var e = window.event;
           if (e.keyCode) code = e.keyCode;
           else if (e.which) code = e.which;
           if (code == 8) {
               e.cancelBubble = true;
               e.returnValue = false;
               removeCharacter();
           } else if (code == 13) {
               e.cancelBubble = true;
               e.returnValue = false;
               addCharacter('<br>')
           }
       }

       function addCharacter(newChar) {
           var fullOutput = '';
           currEditorText.push(newChar);
           for (var i = 0; i < currEditorText.length; i++) {
               fullOutput = fullOutput + currEditorText[i];
           }
           document.getElementById('displayArea').innerHTML = '<table><tr><td class="equation">' + fullOutput + '</td></tr></table>';
       }

       function removeCharacter() {
           currEditorText.pop();
           var fullOutput = '';
           for (var i = 0; i < currEditorText.length; i++) {
               fullOutput = fullOutput + currEditorText[i];
           }
           document.getElementById('displayArea').innerHTML = '<table><tr><td class="equation">' + fullOutput + '</td></tr></table>';
       }

       function addSpecial(type) {
           var text = prompt("Enter the text that you wish to format.", "")
           if (text != null) {
               switch (type) {
                   case 'big':
                       addCharacter('<font size="+2">' + text + '</font>');
                       break;
                   case 'sub':
                       addCharacter('<sub>' + text + '</sub>');
                       break;
                   case 'sup':
                       addCharacter('<sup>' + text + '</sup>');
                       break;
               }
           }
       }

       function clearVal() {
           var startingLength = currEditorText.length;
           for (var i = 0; i < startingLength; i++) {
               currEditorText.pop();
           }
           document.getElementById('displayArea').innerHTML = '';
       }

       function finished() {
           var display = document.getElementById('displayArea');
           var displayColumns = display.getElementsByTagName('td');
           for (var i = 0; i < displayColumns.length; i++) {
               displayColumns[i].className = null;
           }
           window.parent.HardCoreWebEditorPasteContent(display.innerHTML);
           window.parent.CloseEquationEditor();
       }

       function closeWin() {
           window.parent.CloseEquationEditor();
       }
   </script>
   <style type="text/css">
       .buttongroup
        {
	        border-right: black 1px solid;
	        border-top: black 1px solid;
	        border-left: black 1px solid;
	        border-bottom: black 1px solid;
	        font-family: Verdana;
	        background-color: gray;
	        width:auto;
        }

        .button
        {
	        border-right: silver 1px solid;
	        border-top: silver 1px solid;
	        border-left: silver 1px solid;
	        border-bottom: silver 1px solid;
	        font-family: Verdana;
	        background-color: #e5e5e5;
	        font-size: 9pt;
	        width: 15px;
	       text-align: center;
        }

        td.button:hover
        {
	        background-color: #ffffff;
	        cursor: pointer;
	        cursor: hand;
        }

        .equation
        {
	        border-right: gray 1px solid;
	        border-top: gray 1px solid;
	        border-left: gray 1px solid;
	        border-bottom: gray 1px solid;
	        background-color: #e5e5e5;
        }
   </style>
</head>
<body onload="">
<form id="eqedit" action="" method="post">
<center>
<p style="font-family:Verdana; font-size:12pt; font-weight:bold;">Click or type to insert character:</p>
<table>
    <tr>
        <td>
        <div class="buttongroup">
        <table cellspacing="1" cellpadding="2">
            <tr>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#34;</font>')"><font face="Symbol">&#34;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#36;</font>')"><font face="Symbol">&#36;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#216;</font>')"><font face="Symbol">&#216;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#217;</font>')"><font face="Symbol">&#217;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#218;</font>')"><font face="Symbol">&#218;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#199;</font>')"><font face="Symbol">&#199;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#200;</font>')"><font face="Symbol">&#200;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#177;</font>')"><font face="Symbol">&#177;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#180;</font>')"><font face="Symbol">&#180;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#182;</font>')"><font face="Symbol">&#182;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#183;</font>')"><font face="Symbol">&#183;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#215;</font>')"><font face="Symbol">&#215;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#196;</font>')"><font face="Symbol">&#196;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#197;</font>')"><font face="Symbol">&#197;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#184;</font>')"><font face="Symbol">&#184;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#213;</font>')"><font face="Symbol">&#213;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#229;</font>')"><font face="Symbol">&#229;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#214;</font>')"><font face="Symbol">&#214;</font></td>
            </tr>

            <tr>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>\'</font>')"><font face="Symbol">&#39;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#206;</font>')"><font face="Symbol">&#206;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#207;</font>')"><font face="Symbol">&#207;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('&lt;')">&lt;</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('&gt;')">&gt;</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#64;</font>')"><font face="Symbol">&#64;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#94;</font>')"><font face="Symbol">&#94;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#124;</font>')"><font face="Symbol">&#124;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#189;</font>')"><font face="Symbol">&#189;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#163;</font>')"><font face="Symbol">&#163;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#179;</font>')"><font face="Symbol">&#179;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#185;</font>')"><font face="Symbol">&#185;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#186;</font>')"><font face="Symbol">&#186;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#187;</font>')"><font face="Symbol">&#187;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#126;</font>')"><font face="Symbol">&#126;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#198;</font>')"><font face="Symbol">&#198;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#201;</font>')"><font face="Symbol">&#201;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#202;</font>')"><font face="Symbol">&#202;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#203;</font>')"><font face="Symbol">&#203;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#204;</font>')"><font face="Symbol">&#204;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#205;</font>')"><font face="Symbol">&#205;</font></td>
            </tr>

            <tr>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#91;</font>')"><font face="Symbol">&#91;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#93;</font>')"><font face="Symbol">&#93;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#60;</font>')"><font face="Symbol">&#60;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#62;</font>')"><font face="Symbol">&#62;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#123;</font>')"><font face="Symbol">&#123;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#125;</font>')"><font face="Symbol">&#125;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#171;</font>')"><font face="Symbol">&#171;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#172;</font>')"><font face="Symbol">&#172;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#174;</font>')"><font face="Symbol">&#174;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#175;</font>')"><font face="Symbol">&#175;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#219;</font>')"><font face="Symbol">&#219;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#220;</font>')"><font face="Symbol">&#220;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#221;</font>')"><font face="Symbol">&#221;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#222;</font>')"><font face="Symbol">&#222;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#223;</font>')"><font face="Symbol">&#223;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#188;</font>')"><font face="Symbol">&#188;</font></td>
            </tr>

            <tr>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#35;</font>')"><font face="Symbol">&#35;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#37;</font>')"><font face="Symbol">&#37;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#38;</font>')"><font face="Symbol">&#38;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#178;</font>')"><font face="Symbol">&#178;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#96;</font>')"><font face="Symbol">&#96;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#165;</font>')"><font face="Symbol">&#165;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#176;</font>')"><font face="Symbol">&#176;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#190;</font>')"><font face="Symbol">&#190;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#192;</font>')"><font face="Symbol">&#192;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#208;</font>')"><font face="Symbol">&#208;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#209;</font>')"><font face="Symbol">&#209;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('&#916;')">&#916;</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('&cent;')">&cent;</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#210;</font>')"><font face="Symbol">&#210;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#211;</font>')"><font face="Symbol">&#211;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>&#212;</font>')"><font face="Symbol">&#212;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('?')">?</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('.')">.</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter(',')">,</td>
            </tr>

            <tr height=20>
            </tr>

            <tr>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('1')">1</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('2')">2</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('3')">3</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('4')">4</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('5')">5</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('6')">6</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('7')">7</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('8')">8</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('9')">9 </td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('0')">0</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('&frac14;')">&frac14;</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('&frac12;')">&frac12;</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('&frac34;')">&frac34;</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('^')">^</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('¹')">¹</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('²')">²</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('³')">³</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('!')">!</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('#')">#</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('|')">|</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('&acute;')">&acute;</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<sub>^</sub><small>/&macr;</small>')"><sub>^</sub><small>/&macr;</small></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('»')">»</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('«')">«</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('&middot;')">&middot;</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter(':')">:</td>
            </tr>

            <tr>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('+')">+</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('-')">-</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('*')">*</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('×')">×</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('/')">/</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('÷')">÷</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('(')">(</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter(')')">)</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('{')">{</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('}')">}</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('[')">[</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter(']')">]</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<')">&lt;</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('>')">&gt;</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<U><</U>')"><U>&lt;</U></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<U>></U>')"><U>&gt;</U></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('±')">±</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('=')">=</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<U>=</U>')"><U>=</U></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<U>~</U>')"><U>~</U></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>@</font>')"><font face="Symbol">@</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>a</font>')"><font face="Symbol">a</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;><b><BIG>\\</BIG></b></font>')"><font face="Symbol"><B>\</B></font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('Ø')">Ø</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('%')">%</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('°')">°</td>
            </tr>

            <tr>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('a')">a</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('b')">b</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('c')">c</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('d')">d</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('e')">e</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('f')">f</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('g')">g</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('h')">h</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('i')">i</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('j')">j</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('k')">k</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('l')">l</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('m')">m</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('n')">n</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('o')">o</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('p')">p</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('q')">q</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('r')">r</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('s')">s</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('t')">t</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('u')">u</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('v')">v</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('w')">w</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('x')">x</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('y')">y</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('z')">z</td>
            </tr>

            <tr>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('A')">A</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('B')">B</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('C')">C</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('D')">D</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('E')">E</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('F')">F</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('G')">G</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('H')">H</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('I')">I</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('J')">J</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('K')">K</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('L')">L</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('M')">M</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('N')">N</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('O')">O</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('P')">P</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('Q')">Q</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('R')">R</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('S')">S</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('T')">T</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('U')">U</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('V')">V</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('W')">W</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('X')">X</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('Y')">Y</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('Z')">Z</td>
            </tr>

            <tr>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>a</font>')"><font face="Symbol">a</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>b</font>')"><font face="Symbol">b</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>c</font>')"><font face="Symbol">c</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>d</font>')"><font face="Symbol">d</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>e</font>')"><font face="Symbol">e</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>f</font>')"><font face="Symbol">f</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>g</font>')"><font face="Symbol">g</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>h</font>')"><font face="Symbol">h</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>i</font>')"><font face="Symbol">i</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>j</font>')"><font face="Symbol">j</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>k</font>')"><font face="Symbol">k</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>l</font>')"><font face="Symbol">l</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>m</font>')"><font face="Symbol">m</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>n</font>')"><font face="Symbol">n</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>o</font>')"><font face="Symbol">o</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>p</font>')"><font face="Symbol">p</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>q</font>')"><font face="Symbol">q</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>r</font>')"><font face="Symbol">r</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>s</font>')"><font face="Symbol">s</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>t</font>')"><font face="Symbol">t</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>u</font>')"><font face="Symbol">u</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>v</font>')"><font face="Symbol">v</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>w</font>')"><font face="Symbol">w</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>x</font>')"><font face="Symbol">x</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>y</font>')"><font face="Symbol">y</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>z</font>')"><font face="Symbol">z</font></td>
            </tr>

            <tr>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>A</font>')"><span style="font-family: Symbol;">A</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>B</font>')"><font face="Symbol">B</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>C</font>')"><font face="Symbol">C</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>D</font>')"><font face="Symbol">D</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>E</font>')"><font face="Symbol">E</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>F</font>')"><font face="Symbol">F</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>G</font>')"><font face="Symbol">G</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>H</font>')"><font face="Symbol">H</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>I</font>')"><font face="Symbol">I</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>J</font>')"><font face="Symbol">J</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>K</font>')"><font face="Symbol">K</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>L</font>')"><font face="Symbol">L</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>M</font>')"><font face="Symbol">M</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>N</font>')"><font face="Symbol">N</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>O</font>')"><font face="Symbol">O</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>P</font>')"><font face="Symbol">P</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>Q</font>')"><font face="Symbol">Q</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>R</font>')"><font face="Symbol">R</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>S</font>')"><font face="Symbol">S</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>T</font>')"><font face="Symbol">T</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>U</font>')"><font face="Symbol">U</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>V</font>')"><font face="Symbol">V</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>W</font>')"><font face="Symbol">W</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>X</font>')"><font face="Symbol">X</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>Y</font>')"><font face="Symbol">Y</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<font face=&#34;Symbol&#34;>Z</font>')"><font face="Symbol">Z</font></td>
            </tr>
        </table>
        </div>
        </td>
    </tr>
    <tr>
        <td align="center">
        <div class="buttongroup">
         <table cellspacing="1" cellpadding="2">
            <tr>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:removeCharacter()"><font face="verdana">backspace</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('&nbsp;')"><font face="verdana">space</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<BR>')"><font face="verdana">line break</font></td>
                <td align="center">&nbsp;<br />&nbsp;<br /></td> 
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('<HR size=&#34;1&#34;>')"><center><font face="verdana"><u>&nbsp;Dividing&nbsp;</u><br />&nbsp;Line&nbsp;</font></center></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addSpecial('big')"><font face="verdana"><B>BIG</B></font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addSpecial('sub')"><font face="verdana">S<sub>ub</sub></font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addSpecial('sup')"><font face="verdana">S<sup>uper</sup></font></td>
            </tr>
        </table>
        </div>
        </td>
    </tr> 
    <tr>
        <td align="center">
        <div class="buttongroup">
         <table cellspacing="1" cellpadding="2">
            <tr>
                <td align="center"><font face="verdana" size ="-1" color="white">New Section<br>Separator&nbsp; </font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('</td><td class=equation>')">&nbsp;&nbsp;&nbsp;</td> 
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('</td><td class=equation> + </td><td class=equation>')">+</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('</td><td class=equation> - </td><td class=equation>')">-</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('</td><td class=equation> ± </td><td class=equation>')">±</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('</td><td class=equation> = </td><td class=equation>')">=</td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('</td><td class=equation> <font face=&#34;Symbol&#34;>a</font> </td><td class=equation>')"><font face="Symbol">a</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('</td><td class=equation> <u>=</u> </td><td class=equation>')"><u>=</u></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('</td><td class=equation> <font face=&#34;Symbol&#34;>@</font> </td><td class=equation>')"><font face="Symbol">@</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('</td><td class=equation> &lt; </td><td class=equation>')"><font face="verdana" size="-2">&lt;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('</td><td class=equation> &gt; </td><td class=equation>')"><font face="verdana" size="-2">&gt;</font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('</td><td class=equation> <u><</u> </td><td class=equation>')"><font face="verdana" size="-2"><u>&lt;</u></font></td>
                <td align="center" class="button"  style="cursor:pointer;cursor:hand;" onclick="javascript:addCharacter('</td><td class=equation> <u>></u> </td><td class=equation>')"><font face="verdana" size="-2"><u>&gt;</u></font></td>
            </tr>
        </table>
        </div>
        </td>
    </tr>
    <tr>
        <td>
        <div id="displayArea" class="buttongroup" style="width:100%; height:100px; overflow:auto; text-align:center;">&nbsp;</div>
        </td>
    </tr>
</table>

<table>
    <tr>
        <td class="button" style="width:100; height:25; vertical-align:middle; text-align:center; cursor:pointer; cursor:hand;" onclick="finished();">Finished</td>
        <td class="button" style="width:100; height:30; vertical-align:middle; text-align:center; cursor:pointer; cursor:hand;" onclick="clearVal();">Clear</td>
        <td class="button" style="width:100; height:30; vertical-align:middle; text-align:center; cursor:pointer; cursor:hand;" onclick="closeWin()">Cancel</td>
    </tr>
</table>

</center>
</form>
</body>
</html>
