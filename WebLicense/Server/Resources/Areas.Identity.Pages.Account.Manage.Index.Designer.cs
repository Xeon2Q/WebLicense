﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebLicense.Server.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Areas_Identity_Pages_Account_Manage_Index {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Areas_Identity_Pages_Account_Manage_Index() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WebLicense.Server.Resources.Areas.Identity.Pages.Account.Manage.Index", typeof(Areas_Identity_Pages_Account_Manage_Index).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Unexpected error when trying to update profile..
        /// </summary>
        public static string Error_UpdateProfileFailed {
            get {
                return ResourceManager.GetString("Error.UpdateProfileFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Your profile has been updated..
        /// </summary>
        public static string Message_UpdateProfileSuccessful {
            get {
                return ResourceManager.GetString("Message.UpdateProfileSuccessful", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Username.
        /// </summary>
        public static string Model_Name {
            get {
                return ResourceManager.GetString("Model.Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Phone number.
        /// </summary>
        public static string Model_PhoneNumber {
            get {
                return ResourceManager.GetString("Model.PhoneNumber", resourceCulture);
            }
        }
    }
}
