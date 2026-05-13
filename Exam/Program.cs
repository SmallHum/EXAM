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

    public void execute()
    {
    }

    public void setRecieverModule(IModule module)
    {
        this.reciever = module;
    }
}

public class ActivateMessage : AMessage
{
    public ActivateMessage(): base() {}

    public void execute()
    {
        Console.WriteLine("EXEEWIFW");
        reciever.activate();
    }
}

public class DeactivateMessage : AMessage
{
    public DeactivateMessage(): base() {}

    public void execute()
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

    public void execute()
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
    TV
}

public abstract class AModule : IModule
{
    ICentralModule? central_module;
    ModuleType module_type;

    public void activate()
    {
    }

    public void deactivate()
    {
    }

    public void changeValue(float value)
    {
    }

    public float getValue()
    {
        return -100.0f;
    }

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
    public void activate()
    {
        Console.WriteLine("LIGHT TURNED ON");
    }

    public void deactivate()
    {
        Console.WriteLine("LIGHT TURNED OFF");
    }

    public void changeValue(float value)
    {
        brightness = value;
    }
    public float getValue() { return brightness; }
}

public class Furnace : AModule
{
    float temperature;

    public Furnace(float temperature): base(ModuleType.FURNACE)
    {
        this.temperature = temperature;
    }
    public void activate()
    {
        Console.WriteLine("FUENACE TURNED ON");
    }

    public void deactivate()
    {
        Console.WriteLine("FURNACE TURNED OFF");
    }

    public void changeValue(float value)
    {
        temperature = value;
    }
    public float getValue() { return temperature; }
}

public class Program{
    public static void Main(){
        ICentralModule central_module = new CentralModule();



        IModule light = new Light(0.0f);
        IModule furnace = new Furnace(20.0f);

        central_module.registerNewModule(light);
        central_module.registerNewModule(furnace);

        light.sendMessage(ModuleType.FURNACE, new ActivateMessage());


        
    }
}