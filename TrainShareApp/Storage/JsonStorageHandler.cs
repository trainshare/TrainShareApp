using System.Reflection;
using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TrainShareApp.Storage
{
    public abstract class JsonStorageHandler<T> : StorageHandler<T>
    {
        protected StorageInstructionBuilder<T> BuildInstruction(string name, bool deleteAfterResurrect = false)
        {
            StorageInstructionBuilder<T> instruction;

            if (deleteAfterResurrect)
                instruction =
                    AddInstruction()
                        .Configure(
                            x =>
                            {
                                x.Key = name;

                                x.Save =
                                    (instance, getKey, mode) =>
                                    {
                                        var value = JsonConvert.SerializeObject(instance);

                                        x.StorageMechanism.Store(getKey(), value);
                                    };

                                x.Restore =
                                    (instance, getKey, mode) =>
                                    {
                                        object value;
                                        var key = getKey();

                                        if (x.StorageMechanism.TryGet(key, out value) && value is string)
                                        {
                                            JsonConvert.PopulateObject(value as string, instance);
                                            x.StorageMechanism.Delete(key);
                                        }
                                    };
                            });
            else
                instruction =
                    AddInstruction()
                        .Configure(
                            x =>
                            {
                                x.Key = name;

                                x.Save =
                                    (instance, getKey, mode) =>
                                    {
                                        var value = JsonConvert.SerializeObject(instance);

                                        x.StorageMechanism.Store(getKey(), value);
                                    };

                                x.Restore =
                                    (instance, getKey, mode) =>
                                    {
                                        object value;
                                        var key = getKey();

                                        if (x.StorageMechanism.TryGet(key, out value) && value is string)
                                        {
                                            JsonConvert.PopulateObject(value as string, instance);
                                        }
                                    };
                            });

            return instruction;
        }
    }
}
