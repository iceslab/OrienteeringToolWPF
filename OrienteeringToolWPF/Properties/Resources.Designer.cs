﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrienteeringToolWPF.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("OrienteeringToolWPF.Properties.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --
        ///-- Plik wygenerowany przez SQLiteStudio v3.0.7 dnia Pt lut 19 14:55:02 2016
        ///--
        ///-- Użyte kodowanie tekstu: windows-1250
        ///--
        ///PRAGMA foreign_keys = off;
        ///BEGIN TRANSACTION;
        ///
        ///-- Tabela: PROJECT_INFO
        ///CREATE TABLE [PROJECT_INFO] (        ///
        ///[ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT,        ///
        ///[TOURNAMENT_TYPE] INTEGER  NOT NULL        ///
        ///);
        ///
        ///-- Tabela: RELAYS
        ///CREATE TABLE [RELAYS] (        ///
        ///[ID] INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,        ///
        ///[NAME] VARCHAR(255)  UNIQUE NOT NULL        ///
        ///);
        ///
        ///-- Tabela: TOURNAMENT
        ///CREATE TABLE T [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string CreateKCDatabase {
            get {
                return ResourceManager.GetString("CreateKCDatabase", resourceCulture);
            }
        }
    }
}
