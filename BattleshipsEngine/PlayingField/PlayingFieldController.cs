using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;

using Battleships.Boxes;
using Battleships.Exceptions;
using Battleships.Ships;

namespace Battleships.PlayingField
{
    /// <summary>
    /// Spielfeld Controller: Hierüber wird das Spielfeld kontrolliert/verändert/benutzt.
    /// </summary>
    public static class PlayingFieldController
    {
        /// <summary>
        /// Das aktive Spielfeld auf dem operiert wird.
        /// </summary>
        public static PlayingField Active { get; private set; }
        /// <summary>
        /// Wenn das Spielfeld gespeichert oder geladen werden soll, wird die Datei anhand dieses Namens erstellt/geupdated/benutzt.
        /// </summary>
        public static string FileName { get; private set; }

        // Wird zum zufälligen poitionieren der Schiffe verwendet.
        static Random random;
        // Wird zum Überprüfen, ob das Schiff auf dem Spielfeld ist, verwendet.
        static Rectangle playingFieldRec;

        /// <summary>
        /// Setzt das aktive Spielfeld das benutzt werden soll, um beispielsweise Schiffe hinzuzufügen.
        /// </summary>
        /// <param name="playingField">Das zu benutzende Spielfeld.</param>
        public static void Use(PlayingField playingField)
        {
            Active = playingField;

            //
            FileName = string.Empty;
            //

            random = new Random();
            playingFieldRec = new Rectangle(playingField.Location.X, playingField.Location.Y,
                playingField.Size.Width * PlayingField.BoxSize.Width + Drawer.DRAWING_BUG_PX,
                playingField.Size.Height * PlayingField.BoxSize.Height + Drawer.DRAWING_BUG_PX);

            Drawer.Active = playingField;
        }

        static void addBoxesToShip(Ship ship, Point start, ShipDirection direction)
        {
            switch (direction)
            {
                case ShipDirection.Up:
                    ship.Boxes.AddRange(Enumerable.Range(0, ship.Boxes.Capacity)
                        .Select(i => new ShipBox(start.X, start.Y - i * PlayingField.BoxSize.Height)));
                    break;
                case ShipDirection.Down:
                    ship.Boxes.AddRange(Enumerable.Range(0, ship.Boxes.Capacity)
                        .Select(i => new ShipBox(start.X, start.Y + i * PlayingField.BoxSize.Height)));
                    break;
                case ShipDirection.Left:
                    ship.Boxes.AddRange(Enumerable.Range(0, ship.Boxes.Capacity)
                        .Select(i => new ShipBox(start.X - i * PlayingField.BoxSize.Width, start.Y)));
                    break;
                case ShipDirection.Right:
                    ship.Boxes.AddRange(Enumerable.Range(0, ship.Boxes.Capacity)
                        .Select(i => new ShipBox(start.X + i * PlayingField.BoxSize.Width, start.Y)));
                    break;
                default:
                    break;
            }
        }

        static void replaceWaterForShip(Ship ship)
        {
            //TODO...

            List<Rectangle> shipRects = new List<Rectangle>();

            ship.Boxes.ForEach(shipBox => shipRects.Add(shipBox.Rect));

            Active.Water.RemoveAll(waterBox => shipRects.Contains(waterBox.Rect));

            Active.Ships.Add(ship);
        }

        /// <summary>
        /// Versucht ein Schiff an angegebenem Startpunkt in angegebener Richtung zu positionieren.
        /// </summary>
        /// <param name="ship">Das Schiff das platziert werden soll.</param>
        /// <param name="startPosition">Die erste Position des Schiffes (Bug).</param>
        /// <param name="direction">Die Richtung in die das Schiff gebaut werden soll.</param>
        public static bool TryAddingShip(Ship ship, Point startPosition, ShipDirection direction)
        {
            Point start = PlayingFieldHelper.GetBoxPoint(startPosition);

            if (IsOnPlayingField(ship, start, direction))
                addBoxesToShip(ship, start, direction);

            if (IsKnockingToOtherShips(ship, direction))
                return false;
            Active.Ships.Add(ship);
            return true;
        }
        /// <summary>
        /// Platziert ein Schiff an zufälliger Position.
        /// </summary>
        /// <param name="ship">Das Schiff das platziert werden soll.</param>
        public static void AddShipRandomlyPlaced(Ship ship)
        {
            ShipDirection direction;
            do
            {
                Point start = PlayingFieldHelper.AdjustCoordinatesToBoxSize(
                    random.Next(Active.Size.Width * PlayingField.BoxSize.Width - PlayingField.BoxSize.Width),
                    random.Next(Active.Size.Height * PlayingField.BoxSize.Height - PlayingField.BoxSize.Height));
                direction = (ShipDirection)random.Next(4);

                if (IsOnPlayingField(ship, start, direction))
                    addBoxesToShip(ship, start, direction);

            } while (IsKnockingToOtherShips(ship, direction));
            Active.Ships.Add(ship);
        }

        static bool trySerializingPlayingField(string path)
        {
            if (Active != null)
            {
                try
                {
                    var xsSubmit = new DataContractSerializer(typeof(PlayingField));
                    using (XmlWriter writer = XmlWriter.Create(path))
                        xsSubmit.WriteObject(writer, Active);

                    return true;
                }
                catch (Exception ex)
                {
                    //TODO: throw Exception or show MessageBox ?
                    MessageBox.Show(ex.ToString());

                    return false;
                }
            }

            return false;
        }
        static bool tryDeserializingPlayingField(string path)
        {
            try
            {
                var xsSubmit = new DataContractSerializer(typeof(PlayingField));
                using (XmlReader reader = XmlReader.Create(path))
                    Use((PlayingField)xsSubmit.ReadObject(reader));

                return true;
            }
            catch (Exception ex)
            {
                //TODO: throw Exception or show MessageBox ?
                MessageBox.Show(ex.ToString());

                return false;
            }
        }
        /// <summary>
        /// Speichert das aktuelle Spielfeld in einer vorhandenen XML Datei.
        /// </summary>
        public static bool Save()
        {
            if (Active != null && FileName != string.Empty)
                return SaveAs(FileName);
            return false;
        }
        /// <summary>
        /// Speichert das aktuelle Spielfeld als XML in eine Datei.
        /// </summary>
        /// <param name="path">Der Name der Spielfeld XML Datei.</param>
        public static bool SaveAs(string path)
        {
            bool isSerialized = trySerializingPlayingField(path);
            if (isSerialized)
                FileName = path;
            return isSerialized;
        }
        /// <summary>
        /// Lädt ein gespeichertes (als XML formatiertes) Spielfeld und setzt dieses als Aktiv.
        /// </summary>
        /// <param name="path">Der Name der Spielfeld XML Datei.</param>
        public static bool Load(string path)
        {
            bool isDeserialized = tryDeserializingPlayingField(path);
            if (isDeserialized)
                FileName = path;
            return isDeserialized;
        }
        /// <summary>
        /// Schließt das aktuelle Spielfeld und erstellt ein neues leeres Spielfeld.
        /// </summary>
        public static void Close()
        {
            Active = null;
            Drawer.Active = null;

            Drawer.Clear();
        }

        static bool hasPlayingFieldNoShips()
        {
            return Active.Ships.Count == 0;
        }
        static bool hasShipNoBoxes(Ship ship)
        {
            return ship.Boxes.Count == 0;
        }
        static List<Box> createAndGetForbiddenZoneAroundShip(Ship ship, ShipDirection direction)
        {
            var forbiddenZone = new List<Box>();

            int first = -1;
            int last = -1;

            switch (direction)
            {
                case ShipDirection.Up:
                case ShipDirection.Left:
                    first = 0;
                    last = ship.Boxes.Capacity - 1;
                    break;
                case ShipDirection.Down:
                case ShipDirection.Right:
                    first = ship.Boxes.Capacity - 1;
                    last = 0;
                    break;
                default:
                    break;
            }

            switch (direction)
            {
                case ShipDirection.Up:
                case ShipDirection.Down:
                    {
                        var bottomLeft = new ZoneBox(ship.Boxes[first].Rect.X - PlayingField.BoxSize.Width, ship.Boxes[first].Rect.Y + PlayingField.BoxSize.Height);
                        if (bottomLeft.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(bottomLeft);
                        var bottom = new ZoneBox(ship.Boxes[first].Rect.X, ship.Boxes[first].Rect.Y + PlayingField.BoxSize.Height);
                        if (bottom.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(bottom);
                        var bottomRight = new ZoneBox(ship.Boxes[first].Rect.X + PlayingField.BoxSize.Width, ship.Boxes[first].Rect.Y + PlayingField.BoxSize.Height);
                        if (bottomRight.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(bottomRight);

                        var topLeft = new ZoneBox(ship.Boxes[last].Rect.X - PlayingField.BoxSize.Width, ship.Boxes[last].Rect.Y - PlayingField.BoxSize.Height);
                        if (topLeft.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(topLeft);
                        var top = new ZoneBox(ship.Boxes[last].Rect.X, ship.Boxes[last].Rect.Y - PlayingField.BoxSize.Height);
                        if (top.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(top);
                        var topRight = new ZoneBox(ship.Boxes[last].Rect.X + PlayingField.BoxSize.Width, ship.Boxes[last].Rect.Y - PlayingField.BoxSize.Height);
                        if (topRight.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(topRight);
                    }
                    for (int i = 0; i < ship.Boxes.Capacity; i++)
                    {
                        var left = new ZoneBox(ship.Boxes[i].Rect.X - PlayingField.BoxSize.Width, ship.Boxes[i].Rect.Y);
                        if (left.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(left);
                        var right = new ZoneBox(ship.Boxes[i].Rect.X + PlayingField.BoxSize.Width, ship.Boxes[i].Rect.Y);
                        if (right.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(right);
                    }
                    break;
                case ShipDirection.Left:
                case ShipDirection.Right:
                    {
                        var rightTop = new ZoneBox(ship.Boxes[first].Rect.X + PlayingField.BoxSize.Width, ship.Boxes[first].Rect.Y - PlayingField.BoxSize.Height);
                        if (rightTop.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(rightTop);
                        var right = new ZoneBox(ship.Boxes[first].Rect.X + PlayingField.BoxSize.Width, ship.Boxes[first].Rect.Y);
                        if (right.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(right);
                        var rightBottom = new ZoneBox(ship.Boxes[first].Rect.X + PlayingField.BoxSize.Width, ship.Boxes[first].Rect.Y + PlayingField.BoxSize.Height);
                        if (rightBottom.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(rightBottom);

                        var leftTop = new ZoneBox(ship.Boxes[last].Rect.X - PlayingField.BoxSize.Width, ship.Boxes[last].Rect.Y - PlayingField.BoxSize.Height);
                        if (leftTop.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(leftTop);
                        var left = new ZoneBox(ship.Boxes[last].Rect.X - PlayingField.BoxSize.Width, ship.Boxes[last].Rect.Y);
                        if (left.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(left);
                        var leftBottom = new ZoneBox(ship.Boxes[last].Rect.X - PlayingField.BoxSize.Width, ship.Boxes[last].Rect.Y + PlayingField.BoxSize.Height);
                        if (leftBottom.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(leftBottom);
                    }
                    for (int i = 0; i < ship.Boxes.Capacity; i++)
                    {
                        var top = new ZoneBox(ship.Boxes[i].Rect.X, ship.Boxes[i].Rect.Y - PlayingField.BoxSize.Height);
                        if (top.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(top);
                        var bottom = new ZoneBox(ship.Boxes[i].Rect.X, ship.Boxes[i].Rect.Y + PlayingField.BoxSize.Height);
                        if (bottom.Rect.IntersectsWith(playingFieldRec))
                            forbiddenZone.Add(bottom);
                    }
                    break;
                default:
                    break;
            }

            return forbiddenZone;
        }
        static bool isShipHittingForbiddenZone(List<Box> forbiddenZone, Ship ship)
        {
            bool abut = false;
            foreach (var shipPlayingField in Active.Ships)
            {
                foreach (var boxShipPlayingField in shipPlayingField.Boxes)
                {
                    abut = forbiddenZone.Any(f => f.Rect.IntersectsWith(boxShipPlayingField.Rect)) ||
                        ship.Boxes.Any(b => b.Rect.IntersectsWith(boxShipPlayingField.Rect));

                    if (abut)
                    {
                        ship.Boxes.Clear();
                        break;
                    }
                }
                if (abut)
                    break;
            }

            return abut;
        }
        /// <summary>
        /// Überprüft, ob das Schiff an andere Schiffe andockt.
        /// </summary>
        /// <param name="ship">Das Schiff, dass überprüft werden soll.</param>
        /// <param name="direction">Die Richtung, in die das Schiff aufgebaut wird.</param>
        /// <returns>Gibt zurück, ob das Schiff an andere Schiffe andockt.</returns>
        public static bool IsKnockingToOtherShips(Ship ship, ShipDirection direction)
        {
            //Schiff wurde nicht erstellt, weil Schiff noch nicht einmal im gültigen Bereich des Spielfeldes 
            if (hasShipNoBoxes(ship))
                return true;
            //Das erste Schiffe ist immer platzierbar
            if (hasPlayingFieldNoShips())
                return false;
            //Es wird ein unsichtbares Feld um das Schiff erstellt...
            var forbiddenZone = createAndGetForbiddenZoneAroundShip(ship, direction);
            //Prüft, ob das Schiff diese verbotene Zone berührt
            var isHittingZone = isShipHittingForbiddenZone(forbiddenZone, ship);

            return isHittingZone;
        }
        /// <summary>
        /// Überprüft, ob sich das Schiff auf dem Spielfeld befindet.
        /// </summary>
        /// <param name="ship">Das Schiff, dass überprüft werden soll.</param>
        /// <param name="start">Der Start Punkt von dem das Schiff aufgebaut wird.</param>
        /// <param name="direction">Die Richtung, in die das Schiff aufgebaut wird.</param>
        /// <returns>Gibt zurück, ob sich das Schiff auf dem Spielfeld befindet.</returns>
        public static bool IsOnPlayingField(Ship ship, Point start, ShipDirection direction)
        {
            int shipWidth = ship.Boxes.Capacity * PlayingField.BoxSize.Width;
            int shipHeight = ship.Boxes.Capacity * PlayingField.BoxSize.Height;

            return start.X + shipWidth <= Active.Size.Width && direction == ShipDirection.Right ||
                (start.X + PlayingField.BoxSize.Width) - shipWidth >= 0 && direction == ShipDirection.Left ||
                start.Y + shipHeight <= Active.Size.Height && direction == ShipDirection.Down ||
                (start.Y + PlayingField.BoxSize.Height) - shipHeight >= 0 && direction == ShipDirection.Up;
        }
        /// <summary>
        /// Überprüft, ob das Schiff vollständig initialisiert wurde.
        /// </summary>
        /// <param name="ship">Das Schiff, dass überprüft werden soll.</param>
        /// <returns>Gibt zurück, ob das Schiff erfolgreich initialisiert wurde oder nicht.</returns>
        public static bool IsNotFullyInitialized(Ship ship)
        {
            return ship == null ||
                ship.Boxes == null ||
                ship.Boxes[0].Rect == null;
        }

        /// <summary>
        /// Fügt das Schiff direkt zum Spielfeld hinzu.
        /// </summary>
        /// <param name="ship">Das Schiff das hinzugefügt werden soll.</param>
        /// <exception cref="NotInitializedException">Das Schiff wurde nicht vollständig initialisiert.</exception>
        /// <exception cref="NotOnPlayingFieldException">Das Schiff befindet sich nicht auf dem Spielfeld.</exception>
        /// <exception cref="KnockingException">Das Schiff berührt andere Schiffe.</exception>
        public static void AddShipDirectly(Ship ship)
        {
            if (IsNotFullyInitialized(ship))
                throw new NotInitializedException("Ship is not fully initialized!");

            Point start = ship.Boxes[0].Rect.Location;
            ShipDirection direction = PlayingFieldHelper.GetShipDirection(ship);

            if (!IsOnPlayingField(ship, start, direction))
                throw new NotOnPlayingFieldException("Ship is not on playing field!");

            if (IsKnockingToOtherShips(ship, direction))
                throw new KnockingException("Ship is knocking to other ships!");

            replaceWaterForShip(ship);
        }

        /// <summary>
        /// Zeichner: Zeichnet das Spielfeld, Schiffe, Wasser, ...
        /// </summary>
        public static partial class Drawer
        {
            public const short DRAWING_BUG_PX = 1;

            // Stellt das Spielfeld grafisch dar.
            static Panel panel;
            /// <summary>
            /// Das aktive Spielfeld auf dem gezeichnet wird.
            /// </summary>
            public static PlayingField Active { get; set; }

            /// <summary>
            /// The ShipColor is the color that will be used for drawing ships.
            /// </summary>
            public static Color ShipColor { get; set; }
            /// <summary>
            /// The WaterColor is the color that will be used for drawing water.
            /// </summary>
            public static Color WaterColor { get; set; }
            /// <summary>
            /// The BackgroundColor is the color that will be used for the background.
            /// </summary>
            public static Color BackgroundColor { get; set; }
            /// <summary>
            /// The GridColor is the color that will be used for the grid in the background.
            /// </summary>
            public static Color GridColor { get; set; }

            /// <summary>
            /// Stellt die grafische Oberfläsche dar, um beispielsweise darauf Schiffe zeichnen zu können.
            /// </summary>
            /// <param name="panel">Das System.Windows.Forms.Panel das benutzt werden soll um die Grafik anzuzeigen.</param>
            /// <param name="playingfield">Das zu benutzende Spielfeld. Falls nicht gesetzt, wird das Spielfeld des PlayingFieldController benutzt.</param>
            public static void Use(Panel panel, PlayingField playingfield = null)
            {
                if (playingfield != null)
                    Active = playingfield;
                Drawer.panel = panel;

                //
                //Active = PlayingFieldController.Active
                ShipColor = Color.Red;
                WaterColor = Color.Blue;
                BackgroundColor = Color.LightGray;
                GridColor = Color.White;
                //
            }

            static void drawTempShip(Ship ship)
            {
                using (Graphics graphics = panel.CreateGraphics())
                using (Pen green = new Pen(Color.FromArgb(0, 135, 68)))
                using (Pen blue = new Pen(Color.FromArgb(0, 87, 231)))
                using (Pen red = new Pen(Color.FromArgb(214, 45, 32)))
                using (Pen orange = new Pen(Color.FromArgb(255, 167, 0)))
                    if (ship is Cruiser)
                        ship.Boxes.ForEach(i => graphics.DrawRectangle(green, i.Rect));
                    else if (ship is Battleship)
                        ship.Boxes.ForEach(i => graphics.DrawRectangle(blue, i.Rect));
                    else if (ship is UBoat)
                        ship.Boxes.ForEach(i => graphics.DrawRectangle(red, i.Rect));
                    else if (ship is Destroyer)
                        ship.Boxes.ForEach(i => graphics.DrawRectangle(orange, i.Rect));
            }
            /// <summary>
            /// Zeichnet ein temporäres Schiff was dem Benutzer dabei helfen soll, Schiffe leichter anzulegen.
            /// </summary>
            /// <param name="ship">Das Schiff was gezeichnet bzw. später vielleicht auch angelegt werden soll.</param>
            /// <param name="startPosition">Die Start Position von der das Schiff gezeichnet werden soll.</param>
            /// <param name="direction">Die Richtung, in die das Schiff gebaut werden soll.</param>
            /// <returns>Gibt das erzeugte Schiff zurück.</returns>
            public static Ship CreateAndDrawTempShip(Ship ship, Point startPosition, ShipDirection direction)
            {
                Point start = PlayingFieldHelper.GetBoxPoint(startPosition);

                if (IsOnPlayingField(ship, start, direction))
                {
                    addBoxesToShip(ship, start, direction);

                    if (IsKnockingToOtherShips(ship, direction))
                        return null;

                    drawTempShip(ship);

                    return ship;
                }

                return null;
            }
            static void unDrawTempShip(Ship ship)
            {
                using (Graphics graphics = panel.CreateGraphics())
                using (Pen gridColor = new Pen(GridColor))
                    ship.Boxes.ForEach(i => graphics.DrawRectangle(gridColor, i.Rect));
            }
            /// <summary>
            /// Löscht ein temporäre Schiff bzw. übermalt es.
            /// </summary>
            /// <param name="ship">Das Schiff das gelöscht bzw übermalt werden soll.</param>
            /// <returns>Gibt null zurück.</returns>
            public static Ship DeleteAndUnDrawTempShip(Ship ship)
            {
                unDrawTempShip(ship);

                ship = null;

                return ship;
            }
            // Füllt auf dem Panel alle Kästchen mit Wasser aus, die kein Schiff sind.
            static void addWater(Panel panel)
            {
                for (int x = 0; x < panel.Width - DRAWING_BUG_PX; x += PlayingField.BoxSize.Width)
                    for (int y = 0; y < panel.Height - DRAWING_BUG_PX; y += PlayingField.BoxSize.Height)
                        if (!Active.Ships.Any(s => s.Boxes.Any(b => b.Rect.X == x && b.Rect.Y == y)))
                            Active.Water.Add(new WaterBox(x, y));
            }
            // Zeichnet den Hintergrund inklusive Grid.
            static void drawBackground(Panel panel)
            {
                using (Graphics graphics = panel.CreateGraphics())
                {
                    graphics.Clear(BackgroundColor);

                    using (Pen gridColor = new Pen(GridColor))
                    {
                        graphics.DrawRectangle(gridColor, playingFieldRec);
                        for (int x = PlayingField.BoxSize.Width; x < panel.Width; x += PlayingField.BoxSize.Width)
                            graphics.DrawLine(gridColor, x, 0, x, panel.Height);
                        for (int y = PlayingField.BoxSize.Height; y < panel.Height; y += PlayingField.BoxSize.Height)
                            graphics.DrawLine(gridColor, 0, y, panel.Width, y);
                    }
                }
            }
            /// <summary>
            /// Initialisiert die Grafik und alles was dazu gehört um das Spielfeld grafisch aufzubauen und darzustellen.
            /// </summary>
            public static void InitGraphic()
            {
                addWater(panel);

                drawBackground(panel);
            }
            /// <summary>
            /// Füllt das angegebene Kästchen.
            /// </summary>
            /// <param name="box">Das Kästchen das gefüllt werden soll.</param>
            public static void FillBox(Box box)
            {
                using (Graphics graphics = panel.CreateGraphics())
                using (Pen gridColor = new Pen(GridColor))
                {
                    switch (box.Kind)
                    {
                        case BoxKind.Ship:
                            using (Brush shipColor = new SolidBrush(ShipColor))
                                graphics.FillRectangle(shipColor, box.Rect);
                            break;
                        case BoxKind.Water:
                            using (Brush waterColor = new SolidBrush(WaterColor))
                                graphics.FillRectangle(waterColor, box.Rect);
                            break;
                        default:
                            break;
                    }
                    box.IsHit = true;
                    graphics.DrawRectangle(gridColor, box.Rect);
                }
            }
            /// <summary>
            /// Zeichnet alle Schiffe.
            /// </summary>
            /// <param name="state">Der Status: Wenn true, zeichnen, wenn false nicht zeichnen.</param>
            public static void DrawCheat(bool state)
            {
                if (state)
                    Active.Ships.ForEach(drawTempShip);
                else
                    Active.Ships.ForEach(unDrawTempShip);
            }

            static void fillLoadedHittedBoxes()
            {
                using (Pen gridColor = new Pen(GridColor))
                using (Graphics graphics = panel.CreateGraphics())
                {
                    using (Brush shipColor = new SolidBrush(ShipColor))
                        Active.Ships.ForEach(ship => ship.Boxes.ForEach(box =>
                        {
                            if (box.IsHit)
                            {
                                graphics.FillRectangle(shipColor, box.Rect);
                                graphics.DrawRectangle(gridColor, box.Rect);
                            }
                        }));

                    using (Brush waterColor = new SolidBrush(WaterColor))
                        Active.Water.ForEach(box =>
                        {
                            if (box.IsHit)
                            {
                                graphics.FillRectangle(waterColor, box.Rect);
                                graphics.DrawRectangle(gridColor, box.Rect);
                            }
                        });
                }
            }
            /// <summary>
            /// Lädt die Grafik des Spielfeldes. 
            /// </summary>
            public static void LoadGraphic()
            {
                drawBackground(panel);

                fillLoadedHittedBoxes();
            }
            /// <summary>
            /// Löscht die alte Grafik und das Panel.
            /// </summary>
            public static void Clear()
            {
                if (panel != null)
                {
                    using (Graphics graphics = panel.CreateGraphics())
                        graphics.Clear(panel.BackColor);

                    panel = null;
                }
            }
        }
    }
}
