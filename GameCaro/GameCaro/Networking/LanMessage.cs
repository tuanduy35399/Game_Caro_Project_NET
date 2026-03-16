using System;
using System.Globalization;

namespace GameCaro.Networking
{
    public enum LanMessageType
    {
        Hello,
        Move,
        NewGame,
        GameOver,
        Disconnect
    }

    public class LanMessage
    {
        public LanMessageType Type { get; set; }
        public string PlayerName { get; set; }
        public int Col { get; set; }
        public int Row { get; set; }
        public int PlayerIndex { get; set; }
        public int NextPlayerIndex { get; set; }
        public int MoveCount { get; set; }
        public string PlayerX { get; set; }
        public string PlayerO { get; set; }
        public string WinnerName { get; set; }
        public string Reason { get; set; }

        public static LanMessage Hello(string playerName)
        {
            return new LanMessage
            {
                Type = LanMessageType.Hello,
                PlayerName = playerName ?? string.Empty
            };
        }

        public static LanMessage Move(int col, int row, int playerIndex, int nextPlayerIndex, int moveCount)
        {
            return new LanMessage
            {
                Type = LanMessageType.Move,
                Col = col,
                Row = row,
                PlayerIndex = playerIndex,
                NextPlayerIndex = nextPlayerIndex,
                MoveCount = moveCount
            };
        }

        public static LanMessage NewGame(string playerX, string playerO)
        {
            return new LanMessage
            {
                Type = LanMessageType.NewGame,
                PlayerX = playerX ?? string.Empty,
                PlayerO = playerO ?? string.Empty
            };
        }

        public static LanMessage GameOver(string winnerName, string reason, int moveCount)
        {
            return new LanMessage
            {
                Type = LanMessageType.GameOver,
                WinnerName = winnerName ?? string.Empty,
                Reason = reason ?? string.Empty,
                MoveCount = moveCount
            };
        }

        public static LanMessage Disconnect(string reason)
        {
            return new LanMessage
            {
                Type = LanMessageType.Disconnect,
                Reason = reason ?? string.Empty
            };
        }
    }

    public static class LanMessageCodec
    {
        public static string Serialize(LanMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            switch (message.Type)
            {
                case LanMessageType.Hello:
                    return "HELLO|" + Encode(message.PlayerName);

                case LanMessageType.Move:
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "MOVE|{0}|{1}|{2}|{3}|{4}",
                        message.Col,
                        message.Row,
                        message.PlayerIndex,
                        message.NextPlayerIndex,
                        message.MoveCount);

                case LanMessageType.NewGame:
                    return "NEWGAME|" + Encode(message.PlayerX) + "|" + Encode(message.PlayerO);

                case LanMessageType.GameOver:
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "GAMEOVER|{0}|{1}|{2}",
                        Encode(message.WinnerName),
                        Encode(message.Reason),
                        message.MoveCount);

                case LanMessageType.Disconnect:
                    return "DISCONNECT|" + Encode(message.Reason);

                default:
                    throw new InvalidOperationException("Unsupported LAN message type: " + message.Type);
            }
        }

        public static bool TryParse(string line, out LanMessage message)
        {
            message = null;
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            var parts = line.Split('|');
            if (parts.Length == 0)
            {
                return false;
            }

            switch (parts[0].Trim().ToUpperInvariant())
            {
                case "HELLO":
                    if (parts.Length < 2)
                    {
                        return false;
                    }

                    message = LanMessage.Hello(Decode(parts[1]));
                    return true;

                case "MOVE":
                    if (parts.Length < 6)
                    {
                        return false;
                    }

                    int col;
                    int row;
                    int playerIndex;
                    int nextPlayerIndex;
                    int moveCount;
                    if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out col) ||
                        !int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out row) ||
                        !int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out playerIndex) ||
                        !int.TryParse(parts[4], NumberStyles.Integer, CultureInfo.InvariantCulture, out nextPlayerIndex) ||
                        !int.TryParse(parts[5], NumberStyles.Integer, CultureInfo.InvariantCulture, out moveCount))
                    {
                        return false;
                    }

                    message = LanMessage.Move(col, row, playerIndex, nextPlayerIndex, moveCount);
                    return true;

                case "NEWGAME":
                    if (parts.Length < 3)
                    {
                        return false;
                    }

                    message = LanMessage.NewGame(Decode(parts[1]), Decode(parts[2]));
                    return true;

                case "GAMEOVER":
                    if (parts.Length < 4)
                    {
                        return false;
                    }

                    int endMoveCount;
                    if (!int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out endMoveCount))
                    {
                        return false;
                    }

                    message = LanMessage.GameOver(Decode(parts[1]), Decode(parts[2]), endMoveCount);
                    return true;

                case "DISCONNECT":
                    if (parts.Length < 2)
                    {
                        return false;
                    }

                    message = LanMessage.Disconnect(Decode(parts[1]));
                    return true;

                default:
                    return false;
            }
        }

        private static string Encode(string value)
        {
            return Uri.EscapeDataString(value ?? string.Empty);
        }

        private static string Decode(string value)
        {
            return Uri.UnescapeDataString(value ?? string.Empty);
        }
    }
}
