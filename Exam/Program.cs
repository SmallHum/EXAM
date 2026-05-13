public interface IActivatable {
    void activate();
    void deactivate();
}

public interface IRegulatable {
    void changeValue(float newValue);
    float getValue();
}

public interface IModule: IActivatable, IRegulatable {
    void setCentralModule(ICentralModule central_module);

    void sendMessage(ModuleType reciever_type, IMessage message);
    void recieveMessage(IMessage message);
    ICentralModule getCentralModule();

    ModuleType getModuleType();
}

// заведомо неизвестно кто будет получателем команды
// будет гарантированно, что получатель команды будет к моменту исполнения
public interface IMessage
{
    void setRecieverModule(IModule module);
    void execute();
}

public abstract class AMessage : IMessage
{
    protected IModule? reciever;

    public AMessage(){}

    public abstract void execute();

    public void setRecieverModule(IModule module)
    {
        this.reciever = module;
    }
}

public class ActivateMessage : AMessage
{
    public ActivateMessage(): base() {}

    public override void execute()
    {
        reciever.activate();
    }
}

public class DeactivateMessage : AMessage
{
    public DeactivateMessage(): base() {}

    public override void execute()
    {
        reciever.deactivate();
    }
}

public class SetValueMessage : AMessage
{
    protected float value;
    public SetValueMessage(float value): base()
    {
        this.value = value;
    }

    public override void execute()
    {
        reciever.changeValue(value);
    }
}

public interface ICentralModule {
    void registerNewModule(IModule module);
    void sendMessage(ModuleType reciever_type, IMessage message);
}

public class CentralModule: ICentralModule {
    public List<IModule> modules;

    public CentralModule()
    {
        this.modules = new List<IModule>();
    }

    public void registerNewModule(IModule module){
        module.setCentralModule(this);
        modules.Add(module);
    }

    public void sendMessage(ModuleType reciever_type, IMessage message)
    {
        foreach (IModule module in modules)
        {
           if (reciever_type != module.getModuleType()) continue;

           module.recieveMessage(message);
        }
    }
}

// енам чтобы фильтровать получателей сообщений
public enum ModuleType
{
    LIGHT,
    FURNACE,
    MOTION_DETECTOR,
    EMERGENCY_LIGHT,
    TV,
    THERMOMETER
}

public abstract class AModule : IModule
{
    ICentralModule? central_module;
    ModuleType module_type;

    public abstract void activate();

    public abstract void deactivate();

    public abstract void changeValue(float value);

    public abstract float getValue();

    public AModule (ModuleType module_type)
    {
        this.module_type = module_type;
    }

    public ModuleType getModuleType()
    {
        return module_type;
    }

    public ICentralModule getCentralModule()
    {
        return central_module;
    }
    public void setCentralModule(ICentralModule central_module)
    {
        this.central_module = central_module;
    }
    public void sendMessage(ModuleType reciever_type, IMessage message)
    {
        central_module.sendMessage(reciever_type, message);
    }

    public void recieveMessage(IMessage message)
    {
        message.setRecieverModule(this);
        message.execute();
    }
}

public class Light : AModule
{
    float brightness;

    public Light(float brightness): base(ModuleType.LIGHT)
    {
        this.brightness = brightness;
    }
    public override void activate()
    {
        Console.WriteLine("LIGHT TURNED ON");
    }

    public override void deactivate()
    {
        Console.WriteLine("LIGHT TURNED OFF");
    }

    public override void changeValue(float value)
    {
        brightness = value;
    }
    public override float getValue() { return brightness; }
}

public class Furnace : AModule
{
    float temperature;

    public Furnace(float temperature): base(ModuleType.FURNACE)
    {
        this.temperature = temperature;
    }
    public override void activate()
    {
        Console.WriteLine("FUENACE TURNED ON");
    }

    public override void deactivate()
    {
        Console.WriteLine("FURNACE TURNED OFF");
    }

    public override void changeValue(float value)
    {
        Console.WriteLine("REGULATING FURNACE HEAT TO " + value.ToString() + " DEGREES");
        temperature = value;
    }
    public override float getValue() { return temperature; }
}

public class Thermometer : AModule
{
    const float ROOM_TEMPERATURE = 20.0f;
    float measured_temperature;

    public Thermometer(float measured_temperature): base(ModuleType.THERMOMETER)
    {
        this.measured_temperature = measured_temperature;
    }

    public override void activate()
    {
        Console.WriteLine("THERMOMETER ACTIVATED");
    }
    public override void deactivate()
    {
        Console.WriteLine("THERMOMETER DEACTIVATED");
    }
    public override void changeValue(float value)
    {
        Console.WriteLine("THERMOMETER MEASURED " + value.ToString() + " DEGREES");
        this.measured_temperature = value;
        sendMessage(ModuleType.FURNACE, new SetValueMessage(ROOM_TEMPERATURE-measured_temperature));
    }

    public override float getValue()
    {
        return measured_temperature;
    }
}

public class MotionDetector : AModule
{
    float motion_threshold;
    public MotionDetector(float motion_threshold): base(ModuleType.THERMOMETER)
    {
        this.motion_threshold = motion_threshold;
    }

    public override void activate()
    {
        Console.WriteLine("MOTION DETECTER ACTIVATED");
        sendMessage(ModuleType.LIGHT, new ActivateMessage());
    }
    public override void deactivate()
    {
        Console.WriteLine("MOTION DETECTER DEACTIVATED");
    }
    public override void changeValue(float value)
    {
        this.motion_threshold = value;
    }
    public override float getValue()
    {
        return motion_threshold;
    }
}

public class Program{
    public static void Main(){
        ICentralModule central_module = new CentralModule();



        IModule light = new Light(0.0f);
        IModule furnace = new Furnace(20.0f);
        IModule thermometer = new Thermometer(20.0f);
        IModule motion_detector = new MotionDetector(0.5f);

        central_module.registerNewModule(light);
        central_module.registerNewModule(furnace);
        central_module.registerNewModule(thermometer);
        central_module.registerNewModule(motion_detector);

        thermometer.changeValue(10.0f);
        thermometer.changeValue(15.0f);
        thermometer.changeValue(13.0f);

        motion_detector.activate();
        
    }
}