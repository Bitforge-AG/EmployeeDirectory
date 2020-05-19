using System;

using Foundation;
using CallKit;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CallDirectoryExtension.iOS
{
    [Register("CallDirectoryHandler")]
    public class CallDirectoryHandler : CXCallDirectoryProvider, ICXCallDirectoryExtensionContextDelegate
    {
        protected CallDirectoryHandler(IntPtr handle) : base(handle)
        {
            Console.WriteLine("CallDirectoryHandler");
        }

        public override void BeginRequestWithExtensionContext(NSExtensionContext context)
        {
            Console.WriteLine("BeginRequestWithExtensionContext");
            var cxContext = (CXCallDirectoryExtensionContext)context;
            cxContext.Delegate = this;

            if (!AddIdentificationPhoneNumbers(cxContext))
            {
                Console.WriteLine("Unable to add identification phone numbers");
                var error = new NSError(new NSString("CallDirectoryHandler"), 2, null);
                cxContext.CancelRequest(error);
                return;
            }

            cxContext.CompleteRequest(null);
        }

        private bool AddIdentificationPhoneNumbers(
            CXCallDirectoryExtensionContext context)
        {
            var fileIndex = 0;

            var fileName = GetPhoneFilePath(fileIndex);

            int entryIndex = 0;
            // Read all existing files
            while (File.Exists(fileName))
            {
                //Console.WriteLine($"Extension dictionary {fileIndex}");
                // Memory is limited, so flush memory 
                // after each file is read
                using (new NSAutoreleasePool())
                {
                    // The text files have one phone number per line
                    using (StreamReader sr = new StreamReader(fileName))
                    {
                        while (sr.Peek() >= 0)
                        {
                            var phoneString = (sr.ReadLine());
                            // Parse string of format 
                            // "4722334455;Company name"
                            var delimiterIndex = phoneString.IndexOf(';');
                            if (delimiterIndex <= -1
                                || delimiterIndex > phoneString.Length)
                            {
                                continue;
                            }

                            var phoneNumberString =
                              phoneString.Substring(0, delimiterIndex);

                            if (!long.TryParse(phoneNumberString,
                              out long phoneNumber))
                            {
                                continue;
                            }

                            //Console.WriteLine($"AddIdentificationEntry {entryIndex} {phoneNumber} {phoneString.Substring(delimiterIndex + 1)}");
                            // Add phone number to the 
                            // phone's call directory
                            context.AddIdentificationEntry(
                              phoneNumber,
                              phoneString.Substring(delimiterIndex + 1)
                            );
                            entryIndex++;
                        }
                    }
                    // Find filename of next file
                    fileName = GetPhoneFilePath(++fileIndex);
                }
            }
            return true;
        }


        private string GetPhoneFilePath(int index)
        {
            var fileManager = new NSFileManager();
            return Path.Combine(
              fileManager
                .GetContainerUrl("group.com.example.employeedirectory")
                .Path,
              $"phonenumbers.{index}.txt");
        }

        public void RequestFailed(CXCallDirectoryExtensionContext extensionContext, NSError error)
        {
            // An error occurred while adding blocking or identification entries, check the NSError for details.
            // For Call Directory error codes, see the CXErrorCodeCallDirectoryManagerError enum.
            //
            // This may be used to store the error details in a location accessible by the extension's containing app, so that the
            // app may be notified about errors which occured while loading data even if the request to load data was initiated by
            // the user in Settings instead of via the app itself.
            Console.WriteLine($"RequestFailed {error}");
        }
    }
}
