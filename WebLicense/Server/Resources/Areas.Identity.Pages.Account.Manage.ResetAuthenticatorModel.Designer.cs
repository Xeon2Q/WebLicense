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
    public class Areas_Identity_Pages_Account_Manage_ResetAuthenticatorModel {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Areas_Identity_Pages_Account_Manage_ResetAuthenticatorModel() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WebLicense.Server.Resources.Areas.Identity.Pages.Account.Manage.ResetAuthenticato" +
                            "rModel", typeof(Areas_Identity_Pages_Account_Manage_ResetAuthenticatorModel).Assembly);
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
        ///   Looks up a localized string similar to *User with ID &apos;{0}&apos; has reset their authentication app key..
        /// </summary>
        public static string Format_UserDisabledTwoFactorAuthentication {
            get {
                return ResourceManager.GetString("Format.UserDisabledTwoFactorAuthentication", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Your authenticator app key has been reset, you will need to configure your authenticator app using the new key..
        /// </summary>
        public static string Message_AuthenticatorWasReset {
            get {
                return ResourceManager.GetString("Message.AuthenticatorWasReset", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Reset authenticator key.
        /// </summary>
        public static string Text_ResetAuthenticatorKey {
            get {
                return ResourceManager.GetString("Text.ResetAuthenticatorKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Reset authenticator key.
        /// </summary>
        public static string Title {
            get {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *If you reset your authenticator key your authenticator app will not work until you reconfigure it..
        /// </summary>
        public static string Warning_1 {
            get {
                return ResourceManager.GetString("Warning-1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *This process disables 2FA until you verify your authenticator app. If you do not complete your authenticator app configuration you may lose access to your account..
        /// </summary>
        public static string Warning_2 {
            get {
                return ResourceManager.GetString("Warning-2", resourceCulture);
            }
        }
    }
}
