object Form1: TForm1
  Left = 192
  Top = 114
  Width = 804
  Height = 481
  Caption = 'Form1'
  Color = clBlack
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 464
    Top = 360
    Width = 88
    Height = 13
    Caption = 'Initial Trans. Matrix'
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clYellow
    Font.Height = -11
    Font.Name = 'MS Sans Serif'
    Font.Style = []
    ParentFont = False
  end
  object Label2: TLabel
    Left = 576
    Top = 360
    Width = 42
    Height = 13
    Caption = 'Grid Size'
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clYellow
    Font.Height = -11
    Font.Name = 'MS Sans Serif'
    Font.Style = []
    ParentFont = False
  end
  object TetrisEditor1: TTetrisEditor
    Left = 16
    Top = 16
    Width = 100
    Height = 100
    Color = clNavy
    GridSize = 4
    Enabled = True
  end
  object TetrisEditor2: TTetrisEditor
    Left = 128
    Top = 16
    Width = 100
    Height = 100
    Color = clNavy
    GridSize = 4
    Enabled = True
  end
  object TetrisEditor3: TTetrisEditor
    Left = 240
    Top = 16
    Width = 100
    Height = 100
    Color = clNavy
    GridSize = 4
    Enabled = True
  end
  object TetrisEditor4: TTetrisEditor
    Left = 352
    Top = 16
    Width = 100
    Height = 100
    Color = clNavy
    GridSize = 4
    Enabled = True
  end
  object TetrisEditor5: TTetrisEditor
    Left = 464
    Top = 16
    Width = 321
    Height = 321
    Color = clNavy
    GridSize = 10
    Enabled = False
  end
  object ListBox1: TListBox
    Left = 16
    Top = 120
    Width = 433
    Height = 313
    Color = clBlack
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clLime
    Font.Height = -11
    Font.Name = 'MS Sans Serif'
    Font.Style = []
    ItemHeight = 13
    ParentFont = False
    TabOrder = 5
  end
  object ButtonCalc: TButton
    Left = 464
    Top = 408
    Width = 105
    Height = 25
    Caption = 'Calculate'
    TabOrder = 6
    OnClick = ButtonCalcClick
  end
  object SpinEditX: TSpinEdit
    Left = 464
    Top = 376
    Width = 49
    Height = 22
    Color = clBlack
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clYellow
    Font.Height = -11
    Font.Name = 'MS Sans Serif'
    Font.Style = []
    MaxValue = 0
    MinValue = 0
    ParentFont = False
    TabOrder = 7
    Value = 0
  end
  object SpinEditY: TSpinEdit
    Left = 520
    Top = 376
    Width = 49
    Height = 22
    Color = clBlack
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clYellow
    Font.Height = -11
    Font.Name = 'MS Sans Serif'
    Font.Style = []
    MaxValue = 0
    MinValue = 0
    ParentFont = False
    TabOrder = 8
    Value = 0
  end
  object ButtonLoad: TButton
    Left = 664
    Top = 376
    Width = 121
    Height = 25
    Caption = 'Load From File...'
    TabOrder = 9
    OnClick = ButtonLoadClick
  end
  object ButtonSave: TButton
    Left = 664
    Top = 408
    Width = 121
    Height = 25
    Caption = 'Save To File...'
    TabOrder = 10
    OnClick = ButtonSaveClick
  end
  object CheckBox1: TCheckBox
    Left = 712
    Top = 352
    Width = 65
    Height = 17
    Caption = 'Animate'
    Enabled = False
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clYellow
    Font.Height = -11
    Font.Name = 'MS Sans Serif'
    Font.Style = []
    ParentFont = False
    TabOrder = 11
  end
  object SpinEditGridSize: TSpinEdit
    Left = 576
    Top = 376
    Width = 81
    Height = 22
    Color = clBlack
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clYellow
    Font.Height = -11
    Font.Name = 'MS Sans Serif'
    Font.Style = []
    MaxValue = 4
    MinValue = 2
    ParentFont = False
    TabOrder = 12
    Value = 4
    OnChange = SpinEditGridSizeChange
  end
  object Button1: TButton
    Left = 576
    Top = 408
    Width = 81
    Height = 25
    Caption = 'Clear'
    TabOrder = 13
    OnClick = Button1Click
  end
end
