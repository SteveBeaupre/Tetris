unit TetrisEditor;

interface

uses
  Windows, Messages, SysUtils, Classes, Controls, Graphics;

type
 TVec2 = record
  x,y: Integer;
 end;
 TMatrixRow = record
   Row: Array [0..3] of TVec2;
 end;
 TMatrixBlock = record
   Mat: Array [0..3] of TMatrixRow;
 end;

  TTetrisEditor = class(TCustomControl)
  private
    { Private declarations }
    FMouseEnabled: Boolean;
    FGridSize: Integer;
    FClickedBlocks: TStringList;
    procedure SetGridSize(AGridSize: Integer);
    procedure AddClicked(x,y: Integer);
    procedure RemoveClicked(x,y: Integer);
  protected
    { Protected declarations }
    procedure Paint; override;
    procedure WmMouseDown(var Msg : TWMLBUTTONDOWN); Message WM_LBUTTONDOWN;
  public
    { Public declarations }
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;

    procedure Reset;
    function GetNumBlockActivated: Integer;
    function GetCoordinates(): TMatrixRow;
    procedure SetCoordinates(r: TMatrixRow);
  published
    { Published declarations }
    property Align;
    property Canvas;
    property Color;
    property Width;
    property Height;

    property GridSize: Integer read FGridSize write SetGridSize;
    property Enabled: Boolean read FMouseEnabled write FMouseEnabled;
  end;

procedure Register;

implementation

////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////CREATE THE COMPONENT//////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
constructor TTetrisEditor.Create(AOwner: TComponent);
begin
Inherited Create(AOwner);
Width  := 100;
Height := 100;
Color  := clNavy;

FMouseEnabled := true;
FGridSize := 4;

FClickedBlocks := TStringList.Create;
end;
////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////////////////DESTROY THE COMPONENT/////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
destructor TTetrisEditor.Destroy;
begin
FClickedBlocks.Free;
Inherited Destroy;
end;
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
procedure TTetrisEditor.AddClicked(x,y: Integer);
begin
if(not FMouseEnabled) then
  Exit;

if(FClickedBlocks.Count < 4) then begin
  FClickedBlocks.Add(IntToStr(x) + IntToStr(y));
end;
end;
////////////////////////////////////////////////////////////////////////////////////////////
procedure TTetrisEditor.RemoveClicked(x,y: Integer);
var
 i,size: Integer;
begin
if(not FMouseEnabled) then
  Exit;

size := FClickedBlocks.Count;
if(size > 0) then begin

  for i := 0 to size-1 do begin
    if(FClickedBlocks[i] = IntToStr(x) + IntToStr(y)) then begin
      FClickedBlocks.Delete(i);
      break;
    end;
  end;
end;
Paint;
end;
////////////////////////////////////////////////////////////////////////////////////////////
procedure TTetrisEditor.Reset;
begin
FClickedBlocks.Clear;
Paint;
end;
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
function TTetrisEditor.GetCoordinates(): TMatrixRow;
var
 s: String;
 i: Integer;
 r: TMatrixRow;
begin
ZeroMemory(@r, sizeof(TMatrixRow));

if(FClickedBlocks.Count <> 4) then
  Exit;

for i := 0 to FClickedBlocks.Count-1 do begin
  s := FClickedBlocks[i];
  r.Row[i].x := StrToInt(s[1]);
  r.Row[i].y := StrToInt(s[2]);
end;

Result := r;
end;
////////////////////////////////////////////////////////////////////////////////////////////
procedure TTetrisEditor.SetCoordinates(r: TMatrixRow);
var
 i: Integer;
begin
Reset();
for i := 0 to 3 do begin
  if((r.Row[i].x >= 0) and (r.Row[i].x < FGridSize) and (r.Row[i].y >= 0) and (r.Row[i].y < FGridSize)) then begin
    FClickedBlocks.Add(IntToStr(r.Row[i].x) + IntToStr(r.Row[i].y));
  end;
end;
Paint;
end;
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
procedure TTetrisEditor.WmMouseDown(var Msg : TWMLBUTTONDOWN);
var
 x,y,i: Integer;
 s: String;
 Found: Boolean;
begin
x := Msg.XPos div (Self.Width div FGridSize);
y := Msg.YPos div (Self.Height div FGridSize);

Found := False;
for i := 0 to FClickedBlocks.Count-1 do begin
  s := FClickedBlocks[i];
  if((StrToInt(s[1]) = x) and (StrToInt(s[2]) = y)) then begin
    Found := True;
    break;
  end;
end;

if(not Found) then begin
  if(FClickedBlocks.Count < 4) then begin
    AddClicked(x,y);
    Paint;
  end;
end else begin
  RemoveClicked(x,y);
  Paint;
end;
end;
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
function TTetrisEditor.GetNumBlockActivated(): Integer;
begin
Result := FClickedBlocks.Count;
end;
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
procedure TTetrisEditor.SetGridSize(AGridSize: Integer);
var
 i: Integer;
 s: String;
begin
if(AGridSize > 10) then
  AGridSize := 10;
if(AGridSize < 2) then
  AGridSize := 2;

FGridSize := AGridSize;

i := 0;
while(i < FClickedBlocks.Count) do begin
  s := FClickedBlocks[i];
  if((StrToInt(s[1]) < FGridSize) and (StrToInt(s[2]) < FGridSize)) then begin
    Inc(i);
  end else begin
    FClickedBlocks.Delete(i);
  end;
end;

Paint;
end;
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
procedure TTetrisEditor.Paint;
const
 GridSize = 32;
 BorderSize = 2;
var
 r: TRect;
 s: String;
 x,y,z,w,h,i: Integer;
begin
// Erase previous stuffs
Canvas.Brush.Color := $00202020;
Canvas.FillRect(Self.GetClientRect);

// Initialize some variables
w := Width  - (BorderSize*2);
h := Height - (BorderSize*2);

// Draw the main border
Canvas.Pen.Color := $00C0C0C0;
Canvas.MoveTo(BorderSize,   BorderSize);
Canvas.LineTo(BorderSize+w, BorderSize);
Canvas.LineTo(BorderSize+w, BorderSize+h);
Canvas.LineTo(BorderSize,   BorderSize+h);
Canvas.LineTo(BorderSize,   BorderSize);

Canvas.Pen.Color := clGray;
//Canvas.Pen.Style := psDash;
// Draw horizontal dotted lines
for i := 1 to FGridSize-1 do begin
  y := Trunc(BorderSize + ((h / FGridSize) * i));
  Canvas.MoveTo(BorderSize+1,   y);
  Canvas.LineTo(BorderSize+w-1, y);
end;
// Draw vertical dotted lines
for i := 1 to FGridSize-1 do begin
  x := Trunc(BorderSize + ((w / FGridSize) * i));
  Canvas.MoveTo(x, BorderSize+1);
  Canvas.LineTo(x, BorderSize+h-1);
end;
Canvas.Pen.Style := psSolid;

for z := 0 to FClickedBlocks.Count-1 do begin

  s := FClickedBlocks[z];
  x := StrToInt(s[1]);
  y := StrToInt(s[2]);

  r.Left := 1 + (x * (Self.Width div FGridSize));
  r.Top := 1 + (y * (Self.Height div FGridSize));
  r.Right := r.Left + ((Self.Width div FGridSize) - 2);
  r.Bottom := r.Top + ((Self.Height div FGridSize) - 2);

  Canvas.Pen.Color   := clGray;

  case z of
    0: Canvas.Brush.Color := clRed;
    1: Canvas.Brush.Color := clLime;
    2: Canvas.Brush.Color := clBlue;
    3: Canvas.Brush.Color := clYellow;
  end;

  Canvas.Rectangle(r);
end;

Canvas.Pen.Color   := clWhite;
Canvas.Brush.Color := clWhite;
end;
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////
procedure Register;
begin
RegisterComponents('Samples', [TTetrisEditor]);
end;

end.
