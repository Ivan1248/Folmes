﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.0
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'This class was auto-generated by the StronglyTypedResourceBuilder
    'class via a tool like ResGen or Visual Studio.
    'To add or remove a member, edit your .ResX file then rerun ResGen
    'with the /str option, or rebuild your VS project.
    '''<summary>
    '''  A strongly-typed resource class, for looking up localized strings, etc.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Friend Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''  Returns the cached ResourceManager instance used by this class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("Folmes.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Overrides the current thread's CurrentUICulture property for all
        '''  resource lookups using this strongly typed resource class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Looks up a localized resource of type System.Drawing.Bitmap.
        '''</summary>
        Friend ReadOnly Property chat() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("chat", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to document.documentElement.onclick = function (e) {
        '''    e = window.event ? event.srcElement : e.target;
        '''    if (e.className &amp;&amp; e.className.indexOf(&apos;item&apos;) != -1) {
        '''        if (e.style.backgroundColor !== &apos;rgba(255, 0, 0, 0.1)&apos;) {
        '''            e.style.backgroundColor = &apos;rgba(255, 0, 0, 0.1)&apos;;
        '''        } else { e.style.backgroundColor = &apos;transparent&apos;; }
        '''    }
        '''};
        '''document.documentElement.onmouseleave = function () {
        '''    var elements = document.getElementsByClassName(&apos;item&apos;),
        '''        reds = &quot;&quot;,
        '''        i [rest of string was truncated]&quot;;.
        '''</summary>
        Friend ReadOnly Property CleanerScripts() As String
            Get
                Return ResourceManager.GetString("CleanerScripts", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        '''</summary>
        Friend ReadOnly Property DBM() As System.Drawing.Icon
            Get
                Dim obj As Object = ResourceManager.GetObject("DBM", resourceCulture)
                Return CType(obj,System.Drawing.Icon)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized resource of type System.Drawing.Bitmap.
        '''</summary>
        Friend ReadOnly Property menu() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("menu", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized resource of type System.Drawing.Bitmap.
        '''</summary>
        Friend ReadOnly Property newmsg() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("newmsg", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized resource of type System.Drawing.Bitmap.
        '''</summary>
        Friend ReadOnly Property newmsg_online() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("newmsg_online", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized resource of type System.Drawing.Bitmap.
        '''</summary>
        Friend ReadOnly Property online() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("online", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to function getSelectedText() {
        '''    var text = &quot;&quot;;
        '''    if (window.getSelection) {
        '''        text = window.getSelection().toString();
        '''    } else if (document.selection &amp;&amp; document.selection.type != &quot;Control&quot;) {
        '''        text = document.selection.createRange().text;
        '''    }
        '''    return text;
        '''}
        '''
        '''function clickO(inp) {
        '''    var body = document.body;
        '''    body.setAttribute(&quot;data-click&quot;, inp);
        '''    setTimeout(function () {body.setAttribute(&quot;data-click&quot;, &quot;&quot;); }, 100); //ako se poslije klika u prazno
        '''}
        '''
        '''documen [rest of string was truncated]&quot;;.
        '''</summary>
        Friend ReadOnly Property Script() As String
            Get
                Return ResourceManager.GetString("Script", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to var RelMouseY;
        '''var maxPageYOffset;
        '''var scrollTrackSpace;
        '''var scrollerRelMouseY;
        '''var scroller;
        '''
        '''function setScrollerPos(y) {
        '''    scroller.style.top = (y &lt; 0 ? &quot;0&quot; : y &lt; scrollTrackSpace ? y : scrollTrackSpace) + &quot;px&quot;;
        '''}
        '''
        '''function updateScrollerPos() {
        '''    setScrollerPos(Math.round(scrollTrackSpace * window.pageYOffset / maxPageYOffset));
        '''}
        '''
        '''var scrollTarget = 0;
        '''var scrolling = false;
        '''
        '''function smoothScrollToTarget() {
        '''    window.scrollTo(0, (6 * window.pageYOffset + scrollTarget) / 7);
        '''    if (Math [rest of string was truncated]&quot;;.
        '''</summary>
        Friend ReadOnly Property ScrollerScript() As String
            Get
                Return ResourceManager.GetString("ScrollerScript", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to body { background-color:#1e1e1e; color:#a9a9a9; font-family:&quot;Segoe UI&quot;,Arial,sans-serif; margin:0; overflow:hidden; padding-top:2px; word-wrap:break-word; }
        '''img { cursor:pointer; display:block; max-width:100%; }
        '''
        '''::selection { background:rgba(255,255,255,0.1); color:#ddd; }
        '''a:link,a:visited,.url { color:#48b; cursor:pointer; text-decoration:none; transition:color .0s ease; }
        '''a:hover,.url:hover { color:#5ad; cursor:pointer; }
        '''a:active { color:#48b; }
        '''
        '''.file { background-color:#333; border:inset 1px # [rest of string was truncated]&quot;;.
        '''</summary>
        Friend ReadOnly Property Style() As String
            Get
                Return ResourceManager.GetString("Style", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized resource of type System.Drawing.Bitmap.
        '''</summary>
        Friend ReadOnly Property X() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("X", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
    End Module
End Namespace
