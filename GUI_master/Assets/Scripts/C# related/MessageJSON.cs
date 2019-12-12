
/*public class MessageJSON
{
    public string name;

    public MessageContent content;
}

public class MessageContent
{

}


/// <summary>
/// AI_vs_AI gui start
/// </summary>
public class AvA_StartGUI_Message : MessageContent
{
    public string ip;
    public string port;
}*/

public class AckAI_VS_AI_Data
{
    public bool initialBoard { get; set; }
    public float theirRemainingTime { get; set; }
    public float ourRemainingTime { get; set; }
    public int initialCount { get; set; }
}

public class MoveData
{
    public int x { get; set; }
    public int y { get; set; }

    public char color { get; set; }

    public int countCaptured { get; set; }
}

public class GameEndData
{
    public bool win { get; set; }

    public int ourScore { get; set; }
    public int theirScore { get; set; }
}

public class AI_VS_AI_Data
{
    public string IP { get; set; }
    public string port { get; set; }
}

public class Human_VS_AI_Data
{
    public char myColor { get; set; }
    public int initialCount { get; set; }
}

public class RemoveData
{
    public int x { get; set; }
    public int y { get; set; }
}

public class AckData
{
    public bool valid { get; set; }
    public string reason { get; set; }
    public int countCaptured { get; set; }
}