unit MainUnit;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, TetrisEditor, Spin, Clipbrd;

type
  PStringList = ^TStringList;

  TForm1 = class(TForm)
    TetrisEditor1: TTetrisEditor;
    TetrisEditor2: TTetrisEditor;
    TetrisEditor3: TTetrisEditor;
    TetrisEditor4: TTetrisEditor;
    TetrisEditor5: TTetrisEditor;
    ListBox1: TListBox;
    ButtonCalc: TButton;
    Label1: TLabel;
    SpinEditX: TSpinEdit;
    SpinEditY: TSpinEdit;
    ButtonLoad: TButton;
    ButtonSave: TButton;
    CheckBox1: TCheckBox;
    Label2: TLabel;
    SpinEditGridSize: TSpinEdit;
    Button1: TButton;
    procedure ButtonCalcClick(Sender: TObject);
    procedure ButtonSaveClick(Sender: TObject);
    procedure ButtonLoadClick(Sender: TObject);
    procedure SpinEditGridSizeChange(Sender: TObject);
    procedure Button1Click(Sender: TObject);
  private
    { Private declarations }
    InitialPos: TMatrixBlock;
    Rotations: TMatrixBlock;
    InitialTrans: TMatrixRow;
    procedure CalcPosition();
    procedure CalcRotation();
    procedure CalcInitialTranslation;
    procedure PrintMatrixRow(m: TMatrixBlock; row: Integer);
    procedure PrintMatrixRow2(lst: PStringList; name: String; r: TMatrixRow);
    procedure PrintMatrixBlock(BlockName: String; m: TMatrixBlock);
  public
    { Public declarations }
  end;

var
  Form1: TForm1;

implementation

{$R *.dfm}

procedure TForm1.PrintMatrixRow(m: TMatrixBlock; row: Integer);
var
 s: String;
begin
s :=     Format('{%d,%d}, ', [m.Mat[row].Row[0].x, m.Mat[row].Row[0].y]);
s := s + Format('{%d,%d}, ', [m.Mat[row].Row[1].x, m.Mat[row].Row[1].y]);
s := s + Format('{%d,%d}, ', [m.Mat[row].Row[2].x, m.Mat[row].Row[2].y]);
s := s + Format('{%d,%d}',   [m.Mat[row].Row[3].x, m.Mat[row].Row[3].y]);
Listbox1.Items.Add(s);
end;

procedure TForm1.PrintMatrixBlock(BlockName: String; m: TMatrixBlock);
var
 i: Integer;
begin
Listbox1.Items.Add(BlockName + ' = {');
for i := 0 to 3 do
  PrintMatrixRow(m, i);
Listbox1.Items.Add('}');
end;

procedure TForm1.PrintMatrixRow2(lst: PStringList; name: String; r: TMatrixRow);
var
 s: String;
begin
s := Format('%s', [name]);
s := s + Format('new Vec2(%d,%d), ', [r.Row[0].x, r.Row[0].y]);
s := s + Format('new Vec2(%d,%d), ', [r.Row[1].x, r.Row[1].y]);
s := s + Format('new Vec2(%d,%d), ', [r.Row[2].x, r.Row[2].y]);
s := s + Format('new Vec2(%d,%d)',   [r.Row[3].x, r.Row[3].y]);
s := s + ');';
lst.Add(s);
end;

procedure TForm1.CalcRotation();
var
 x,y,z: Integer;
begin

ZeroMemory(@Rotations, sizeof(TMatrixBlock));

for y := 0 to 3-1 do begin
  for x := 0 to 3 do begin
    if(y + 1 = 4) then begin
      z := 0;
    end else begin
      z := y + 1;
    end;
    Rotations.Mat[y+1].Row[x].x := InitialPos.Mat[z].Row[x].x - InitialPos.Mat[y].Row[x].x;
    Rotations.Mat[y+1].Row[x].y := InitialPos.Mat[z].Row[x].y - InitialPos.Mat[y].Row[x].y;
  end;
end;
end;

procedure TForm1.CalcPosition;
begin
InitialPos.Mat[0] := TetrisEditor1.GetCoordinates();
InitialPos.Mat[1] := TetrisEditor2.GetCoordinates();
InitialPos.Mat[2] := TetrisEditor3.GetCoordinates();
InitialPos.Mat[3] := TetrisEditor4.GetCoordinates();
end;

procedure TForm1.CalcInitialTranslation;
var
 x,y,i: Integer;
begin
InitialTrans := TetrisEditor1.GetCoordinates();

for i := 0 to 3 do begin
  x := InitialTrans.Row[i].x;
  y := InitialTrans.Row[i].y;

  if((x >= 0) and (x <= 3) and (y >= 0) and (y <= 3)) then begin
    InitialTrans.Row[i].x := x + SpinEditX.Value;
    InitialTrans.Row[i].y := y + SpinEditY.Value;
  end else begin
    InitialTrans.Row[i].x := -1;
    InitialTrans.Row[i].y := -1;
  end;
end;

TetrisEditor5.SetCoordinates(InitialTrans);
end;

procedure TForm1.ButtonCalcClick(Sender: TObject);
var
 i: Integer;
 Clipbrd: TClipBoard;
 StrLst: TStringList;
begin
if((TetrisEditor1.GetNumBlockActivated = 4) and
    (TetrisEditor2.GetNumBlockActivated = 4) and
    (TetrisEditor3.GetNumBlockActivated = 4) and
    (TetrisEditor4.GetNumBlockActivated = 4)) then begin
    Listbox1.Clear;
    CalcPosition;
    CalcRotation;
    CalcInitialTranslation();

    PrintMatrixBlock('Rotation Matrices', Rotations);
    PrintMatrixBlock('Initial Position', InitialPos);

    Clipbrd := TClipBoard.Create();
    StrLst := TStringList.Create();
    StrLst.Add('');
    StrLst.Add('***OUTPUT***');
    StrLst.Add('(Results copied to clipboard...)');
    PrintMatrixRow2(@StrLst, 'RotationMatrix['+IntToStr(0) + '].set(', Rotations.Mat[0]);
    PrintMatrixRow2(@StrLst, 'RotationMatrix['+IntToStr(1) + '].set(', Rotations.Mat[1]);
    PrintMatrixRow2(@StrLst, 'RotationMatrix['+IntToStr(2) + '].set(', Rotations.Mat[2]);
    PrintMatrixRow2(@StrLst, 'RotationMatrix['+IntToStr(3) + '].set(', Rotations.Mat[3]);
    StrLst.Add('');
    PrintMatrixRow2(@StrLst, 'InitialBlockPosition = new TetrisMatrix(', InitialPos.Mat[0]);
    StrLst.Add('InitialTranslationMatrix = new TetrisMatrix(new Vec2(' + IntToStr(SpinEditX.Value) + ', ' + IntToStr(SpinEditY.Value) + '));');
    StrLst.Add('');
    ListBox1.Items.AddStrings(StrLst);
    for i := 0 to 2 do
      StrLst.Delete(0);
    Clipbrd.AsText := StrLst.Text;
    StrLst.Text;

    StrLst.Free;
    Clipbrd.Free;
end else begin
  ShowMessage('You''re missing some blocks!');
end;
end;

procedure TForm1.ButtonSaveClick(Sender: TObject);
var
 f: FILE;
 SavDlg: TSaveDialog;
 NumBytesToWrite, NumBytesWritten: Integer;
 m: TMatrixBlock;
 tx,ty,gs: Integer;
begin
if((TetrisEditor1.GetNumBlockActivated = 4) and
    (TetrisEditor2.GetNumBlockActivated = 4) and
    (TetrisEditor3.GetNumBlockActivated = 4) and
    (TetrisEditor4.GetNumBlockActivated = 4)) then begin

     SavDlg := TSaveDialog.Create(Self);
     SavDlg.Options := SavDlg.Options + [ofOverwritePrompt];
     SavDlg.Filter := 'Setting files (*.bin)|*.bin';
     SavDlg.FilterIndex := 0;
     SavDlg.DefaultExt := 'bin';

     if(SavDlg.Execute) then begin
       AssignFile(f, SavDlg.FileName);
       Rewrite(f,1);

       m.Mat[0] := TetrisEditor1.GetCoordinates();
       m.Mat[1] := TetrisEditor2.GetCoordinates();
       m.Mat[2] := TetrisEditor3.GetCoordinates();
       m.Mat[3] := TetrisEditor4.GetCoordinates();

       tx := SpinEditX.Value;
       ty := SpinEditY.Value;
       gs := SpinEditGridSize.Value;

       NumBytesToWrite := sizeof(TMatrixBlock);
       BlockWrite(f, m, NumBytesToWrite, NumBytesWritten);

       NumBytesToWrite := sizeof(Integer);
       BlockWrite(f, tx, NumBytesToWrite, NumBytesWritten);
       BlockWrite(f, ty, NumBytesToWrite, NumBytesWritten);
       BlockWrite(f, gs, NumBytesToWrite, NumBytesWritten);

       BlockWrite(f, ty, NumBytesToWrite, NumBytesWritten);

       CloseFile(f);
     end;
end;
end;

procedure TForm1.ButtonLoadClick(Sender: TObject);
var
 f: FILE;
 OpnDlg: TOpenDialog;
 NumBytesToRead, NumBytesRead: Integer;
 m: TMatrixBlock;
 tx,ty,gs: Integer;
begin
OpnDlg := TOpenDialog.Create(Self);
OpnDlg.Filter := 'Setting files (*.bin)|*.bin';
OpnDlg.FilterIndex := 0;
OpnDlg.DefaultExt := 'bin';

if(OpnDlg.Execute) then begin
  AssignFile(f, OpnDlg.FileName);
  Reset(f,1);

  NumBytesToRead := sizeof(TMatrixBlock);
  BlockRead(f, m, NumBytesToRead, NumBytesRead);

  NumBytesToRead := sizeof(Integer);
  BlockRead(f, tx, NumBytesToRead, NumBytesRead);
  BlockRead(f, ty, NumBytesToRead, NumBytesRead);
  BlockRead(f, gs, NumBytesToRead, NumBytesRead);

  CloseFile(f);

  SpinEditX.Value := tx;
  SpinEditY.Value := ty;
  SpinEditGridSize.Value := gs;
  SpinEditGridSize.OnChange(Self);

  TetrisEditor1.SetCoordinates(m.Mat[0]);
  TetrisEditor2.SetCoordinates(m.Mat[1]);
  TetrisEditor3.SetCoordinates(m.Mat[2]);
  TetrisEditor4.SetCoordinates(m.Mat[3]);
  TetrisEditor5.Reset();

  ListBox1.Clear;
end;
end;

procedure TForm1.SpinEditGridSizeChange(Sender: TObject);
begin
TetrisEditor1.GridSize := SpinEditGridSize.Value;
TetrisEditor2.GridSize := SpinEditGridSize.Value;
TetrisEditor3.GridSize := SpinEditGridSize.Value;
TetrisEditor4.GridSize := SpinEditGridSize.Value;
end;

procedure TForm1.Button1Click(Sender: TObject);
begin
ZeroMemory(@InitialPos, sizeof(TMatrixBlock));
ZeroMemory(@Rotations, sizeof(TMatrixBlock));
ZeroMemory(@InitialTrans, sizeof(TMatrixRow));
TetrisEditor1.Reset();
TetrisEditor2.Reset();
TetrisEditor3.Reset();
TetrisEditor4.Reset();
TetrisEditor5.Reset();
ListBox1.Clear;
end;

end.
