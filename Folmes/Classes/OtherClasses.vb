Class ToolstripColorTable
    Inherits ProfessionalColorTable

    Public Overrides ReadOnly Property CheckBackground As Color
        Get
            Return Color.FromArgb(255, 255, 255)
        End Get
    End Property
    'Border
    Public Overrides ReadOnly Property ToolStripBorder As Color
        Get
            Return Color.FromArgb(51, 51, 51)
        End Get
    End Property
    'ButtonSelected
    Public Overrides ReadOnly Property ButtonSelectedGradientBegin As Color
        Get
            Return Color.FromArgb(68, 68, 68)
        End Get
    End Property

    Public Overrides ReadOnly Property ButtonSelectedGradientMiddle As Color
        Get
            Return Color.FromArgb(68, 68, 68)
        End Get
    End Property

    Public Overrides ReadOnly Property ButtonSelectedGradientEnd As Color
        Get
            Return Color.FromArgb(68, 68, 68)
        End Get
    End Property
    '   Border
    Public Overrides ReadOnly Property ButtonSelectedBorder As Color
        Get
            Return Color.FromArgb(68, 68, 68)
        End Get
    End Property

    'ButtonPressed
    Public Overrides ReadOnly Property ButtonPressedGradientBegin As Color
        Get
            Return Color.FromArgb(85, 85, 85)
        End Get
    End Property

    Public Overrides ReadOnly Property ButtonPressedGradientMiddle As Color
        Get
            Return Color.FromArgb(85, 85, 85)
        End Get
    End Property

    Public Overrides ReadOnly Property ButtonPressedGradientEnd As Color
        Get
            Return Color.FromArgb(85, 85, 85)
        End Get
    End Property
    '   Border
    Public Overrides ReadOnly Property ButtonPressedBorder As Color
        Get
            Return Color.FromArgb(0, 64, 64, 64)
        End Get
    End Property
    '   MenuButtonPressed
    Public Overrides ReadOnly Property MenuItemPressedGradientBegin As Color
        Get
            Return Color.FromArgb(34, 34, 34)
        End Get
    End Property

    Public Overrides ReadOnly Property MenuItemPressedGradientEnd As Color
        Get
            Return Color.FromArgb(34, 34, 34)
        End Get
    End Property
    '       Border
    Public Overrides ReadOnly Property MenuBorder As Color
        Get
            Return Color.FromArgb(68, 68, 68)
        End Get
    End Property
    'Menu
    Public Overrides ReadOnly Property ToolStripDropDownBackground As Color
        Get
            Return Color.FromArgb(34, 34, 34)
        End Get
    End Property
    '   ImageMargin 
    Public Overrides ReadOnly Property ImageMarginGradientBegin As Color
        Get
            Return Color.FromArgb(34, 34, 34)
        End Get
    End Property

    Public Overrides ReadOnly Property ImageMarginGradientMiddle As Color
        Get
            Return Color.FromArgb(34, 34, 34)
        End Get
    End Property

    Public Overrides ReadOnly Property ImageMarginGradientEnd As Color
        Get
            Return Color.FromArgb(34, 34, 34)
        End Get
    End Property
    '   MenuItem
    Public Overrides ReadOnly Property MenuItemSelected As Color
        Get
            Return Color.FromArgb(51, 51, 51)
        End Get
    End Property
    '       Border
    Public Overrides ReadOnly Property MenuItemBorder As Color
        Get
            Return Color.FromArgb(34, 34, 34)
        End Get
    End Property
    '   Separator
    Public Overrides ReadOnly Property SeparatorDark As Color
        Get
            Return Color.FromArgb(68, 68, 68)
        End Get
    End Property
End Class
