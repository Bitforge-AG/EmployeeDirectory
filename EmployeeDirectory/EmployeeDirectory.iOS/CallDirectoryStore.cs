using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CallDirectoryExtension.iOS;
using CallKit;
using EmployeeDirectory;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(CallDirectoryStore))]
namespace CallDirectoryExtension.iOS
{
    public class CallDirectoryStore : ICallDirectoryStore
    {
        public CallDirectoryStore()
        {
        }

        public async Task<RegistrationResponse> Store(IOrderedEnumerable<KeyValuePair<long, string>> sorted)
        {
            var arr = sorted.ToArray();
            const int EntriesPerDictionary = 100000; //create separate files for subsets of entries
            const string delimiter = ";";

            for (int i = 0; i < 1000; i++)
            {

                var fileName = GetPhoneFilePath(i);

                // Read all existing files
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                else
                {
                    //Console.WriteLine("Deleted " + i + " files");
                    break;
                }
            }

            for (int i=0; i < arr.Length / EntriesPerDictionary+1; i++){

                var fileName = GetPhoneFilePath(i);
                //var dictionary = new NSMutableDictionary();
                var lines = new List<string>();

                for (int j = 0; j<EntriesPerDictionary && j+i* EntriesPerDictionary < arr.Length; j++)
                {
                    var entry = arr[j + i * EntriesPerDictionary];
                    lines.Add($"{entry.Key}{delimiter}{entry.Value}");
                }
                File.WriteAllLines(fileName, lines.ToArray());


                Console.WriteLine($"AddIdentificationEntry fileName {fileName}");
                //Console.WriteLine("PhonenumberDirectory" + i);

            }

            var taskCompletionSource = new TaskCompletionSource<RegistrationResponse>();
            var task = taskCompletionSource.Task;
            //Action<RegistrationResponse> callback = taskCompletionSource.SetResult;


            CXCallDirectoryManager.SharedInstance.ReloadExtension(
                "com.example.employeedirectory.callkitextension", 
                error => 
                { 
                    if (error == null) 
                    {
                        // Reloaded extension successfully
                        taskCompletionSource.SetResult(new RegistrationResponse { Response = RegistrationResponse.RegistrationResponseEnum.Ok });
                    } else {
                        switch ((CXErrorCodeCallDirectoryManagerError)(int)error.Code)
                        {
                            case CXErrorCodeCallDirectoryManagerError.ExtensionDisabled:
                                taskCompletionSource.SetResult(new RegistrationResponse { Response = RegistrationResponse.RegistrationResponseEnum.NotActivated });
                                return;
                            case CXErrorCodeCallDirectoryManagerError.Unknown:
                            case CXErrorCodeCallDirectoryManagerError.NoExtensionFound:
                            case CXErrorCodeCallDirectoryManagerError.LoadingInterrupted:
                            case CXErrorCodeCallDirectoryManagerError.EntriesOutOfOrder:
                            case CXErrorCodeCallDirectoryManagerError.DuplicateEntries:
                            case CXErrorCodeCallDirectoryManagerError.MaximumEntriesExceeded:
                            case CXErrorCodeCallDirectoryManagerError.CurrentlyLoading:
                            case CXErrorCodeCallDirectoryManagerError.UnexpectedIncrementalRemoval:
                                Console.WriteLine(error);
                                taskCompletionSource.SetResult(new RegistrationResponse {
                                    Response =
                                    RegistrationResponse.RegistrationResponseEnum.Error,
                                    Reason = "" + error.Code
                                });
                                return;
                        }
                        taskCompletionSource.SetResult(
                            new RegistrationResponse {
                                Response = RegistrationResponse.RegistrationResponseEnum.Error,
                                Reason = ""+error.Code
                            });
                    }
                }
            );

            return await Task.FromResult(task.Result);
        }

        private string GetPhoneFilePath(int index)
        {
            var fileManager = new NSFileManager();
            var path1 = fileManager
                .GetContainerUrl("group.com.example.employeedirectory")?
                .Path;
            var path2 = $"phonenumbers.{index}.txt";

            if (path1 == null) return path2;

            return Path.Combine(
              path1,
              path2);
        }
    }
}
