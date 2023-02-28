using System.Runtime.InteropServices;
public class Timer
{
    private DateTime _currentTime;
    private DateTime? _setTime = null;
    public Timer() => _currentTime = DateTime.Now;
    
    public DateTime NowTime() => _currentTime;

    public bool IsGoing() => _currentTime < _setTime;

    public void SetTime(int minutes) => _setTime = _currentTime.AddMinutes(minutes);

    public void Update() => _currentTime = DateTime.Now;
}

static class Input
{
    private static DateTime updateTime;

    private static DateTime lastTimePressed;

    private static bool isFirstTimePressed = true;

    public static void SetTime()
    {
        updateTime = DateTime.Now;
    }

    [DllImport("user32.dll")]
    public static extern int GetAsyncKeyState(KeyCode nVirtKey);

    public enum KeyCode
    {
        Space = 0x20,
        ESC = 0x1B,
        ArrowLeft = 0x25,
        ArrowRight = 0x27,
        Enter = 0x0D
    }

    public static bool IsKeyPressed(KeyCode key)
    {
        if (isFirstTimePressed && GetAsyncKeyState(key) > 0)
        {
            isFirstTimePressed = false;
            lastTimePressed = DateTime.Now;
            return true;
        }

        if (GetAsyncKeyState(key) > 0 &&
            updateTime.Second > lastTimePressed.Second || updateTime.Minute > lastTimePressed.Minute
            || updateTime.Hour > lastTimePressed.Hour)
        {
            lastTimePressed = DateTime.Now;
            return true;
        }

        return false;
    }
}

static class State
{
   private static Graphics graphics = new Graphics();
   public static int minutes { get; set; }
   public static void StartUp(ref Timer timer)
   {
        minutes = 5;
        graphics.AwakeSession();
        bool isStarted = false;

        while (!isStarted)
        {
            Input.SetTime();

            if (Input.IsKeyPressed(Input.KeyCode.ArrowLeft) && minutes > 5)
            {
                minutes -= 5;
                graphics.RefreshStartUpScreen();
            }
            
            else if (Input.IsKeyPressed(Input.KeyCode.ArrowRight) && minutes < 60)
            {
                minutes += 5;
                graphics.RefreshStartUpScreen();
            }
            
            else if (Input.IsKeyPressed(Input.KeyCode.Enter))
            {
                isStarted = true;
                StartSession(ref timer);
            }

            timer.Update();
        }
   }

    private static void StartSession(ref Timer timer)
    {
        timer.SetTime(minutes);

        while (timer.IsGoing())
        {
            timer.Update();
            graphics.SessionDraw(ref timer);
        }
    }

}
public class Graphics
{ 
    public void AwakeSession()
    {
        Console.CursorVisible = false;
        Console.Title = "Pomodoro";
        Console.SetWindowSize(30, 5);
        
        Console.SetCursorPosition(10, 0);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Pomodoro");

        DrawMinutesChoice();
    }

    private void DrawMinutesChoice()
    {
        Console.SetCursorPosition(7, 1);
        Console.Write(">>   ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(State.minutes + ":00");

        Console.SetCursorPosition(5, 2);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Press ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Enter ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("to start");
    }

    public void RefreshStartUpScreen()
    {
        Console.Clear();
        AwakeSession();
    }

    public void SessionDraw(ref Timer time)
    {
        Console.Clear();

        
        
    }
}
class Execute
{
    static void Main()
    {
        bool isRunning = true;

        Timer timer = new();

        State.StartUp(ref timer);

        while (isRunning)
        {
            timer.Update();
        }
    }
}





