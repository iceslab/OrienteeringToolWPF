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
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:d-M-yyyy}.
        /// </summary>
        public static string BirthDateFormat {
            get {
                return ResourceManager.GetString("BirthDateFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nie odnaleziono stacji.
        /// </summary>
        public static string CannotFindStation {
            get {
                return ResourceManager.GetString("CannotFindStation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kategoria.
        /// </summary>
        public static string CategoryName {
            get {
                return ResourceManager.GetString("CategoryName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Czas kontroli: {0:H:mm:ss}.
        /// </summary>
        public static string CheckTimeFormat {
            get {
                return ResourceManager.GetString("CheckTimeFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Numer chipa: {0}.
        /// </summary>
        public static string ChipNumberFormat {
            get {
                return ResourceManager.GetString("ChipNumberFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nie można rozpocząć zakończonych zawodów..
        /// </summary>
        public static string CompetitionCannotStart {
            get {
                return ResourceManager.GetString("CompetitionCannotStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Zawody zostały zakończone.
        /// </summary>
        public static string CompetitionHasFinished {
            get {
                return ResourceManager.GetString("CompetitionHasFinished", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Data urodzenia.
        /// </summary>
        public static string CompetitorBirthDate {
            get {
                return ResourceManager.GetString("CompetitorBirthDate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kategoria.
        /// </summary>
        public static string CompetitorCategory {
            get {
                return ResourceManager.GetString("CompetitorCategory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Chip.
        /// </summary>
        public static string CompetitorChip {
            get {
                return ResourceManager.GetString("CompetitorChip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Imię i nazwisko.
        /// </summary>
        public static string CompetitorName {
            get {
                return ResourceManager.GetString("CompetitorName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sztafeta.
        /// </summary>
        public static string CompetitorRelay {
            get {
                return ResourceManager.GetString("CompetitorRelay", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Aby móc rozpocząć lub kontynuować zawody należy połączyć się ze stacją.
        ///Spróbować ponownie?.
        /// </summary>
        public static string ConnectionNeeddedInfo {
            get {
                return ResourceManager.GetString("ConnectionNeeddedInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nie połączono się ze stacją.
        /// </summary>
        public static string ConnectionNotConnected {
            get {
                return ResourceManager.GetString("ConnectionNotConnected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Naciśnij &quot;Wyszukaj&quot; aby znaleźć stację.
        /// </summary>
        public static string ConnectionWindowUsage {
            get {
                return ResourceManager.GetString("ConnectionWindowUsage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to --
        ///-- Plik wygenerowany przez SQLiteStudio v3.0.7 dnia Pn wrz 5 19:31:30 2016
        ///--
        ///-- Użyte kodowanie tekstu: windows-1250
        ///--
        ///PRAGMA foreign_keys = off;
        ///BEGIN TRANSACTION;
        ///
        ///-- Tabela: TOURNAMENT
        ///DROP TABLE IF EXISTS TOURNAMENT;
        ///CREATE TABLE TOURNAMENT (ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, START_TIME DATETIME NOT NULL, STARTED_AT_TIME DATETIME, FINISHED_AT_TIME DATETIME, NAME VARCHAR (255) NOT NULL, COURSE_TYPE INTEGER NOT NULL, DESCRIPTION TEXT);
        ///
        ///-- Tabela: PROJECT_INFO
        ///DROP TABLE IF E [rest of string was truncated]&quot;;.
        /// </summary>
        public static string CreateKCDatabase {
            get {
                return ResourceManager.GetString("CreateKCDatabase", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Uwaga!
        ///Usunięte zostaną także wszystkie powiązane dane..
        /// </summary>
        public static string DatabaseCascadeDeleteWarning {
            get {
                return ResourceManager.GetString("DatabaseCascadeDeleteWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Baza danych SQLite3|*.sqlite;*.db|Wszystkie pliki (*.*)|*.*.
        /// </summary>
        public static string DatabaseDialogFilters {
            get {
                return ResourceManager.GetString("DatabaseDialogFilters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Uwaga!
        ///Baza danych istnieje, czy chcesz nadpisać?.
        /// </summary>
        public static string DatabaseOverwriteWarning {
            get {
                return ResourceManager.GetString("DatabaseOverwriteWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:##0}:{1:00}.
        /// </summary>
        public static string DeltaTimeFormat {
            get {
                return ResourceManager.GetString("DeltaTimeFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Błędy:.
        /// </summary>
        public static string Errors {
            get {
                return ResourceManager.GetString("Errors", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Czas mety: {0:H:mm:ss}.
        /// </summary>
        public static string FinishTimeFormat {
            get {
                return ResourceManager.GetString("FinishTimeFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wybrana kategoria jest nieprawidłowa.
        /// </summary>
        public static string InvalidCategoryError {
            get {
                return ResourceManager.GetString("InvalidCategoryError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wybrany typ sztafety jest nieprawidłowy.
        /// </summary>
        public static string InvalidCourseTypeError {
            get {
                return ResourceManager.GetString("InvalidCourseTypeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wprowadzono nieprawidłowe dane.
        /// </summary>
        public static string InvalidDataTitle {
            get {
                return ResourceManager.GetString("InvalidDataTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Podana data jest nieprawidłowa.
        /// </summary>
        public static string InvalidDateError {
            get {
                return ResourceManager.GetString("InvalidDateError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wybrana kolejność jest nieprawidłowa.
        /// </summary>
        public static string InvalidOrderError {
            get {
                return ResourceManager.GetString("InvalidOrderError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wybrana sztafeta jest nieprawidłowa.
        /// </summary>
        public static string InvalidRelayError {
            get {
                return ResourceManager.GetString("InvalidRelayError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Brak.
        /// </summary>
        public static string None {
            get {
                return ResourceManager.GetString("None", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:H:mm:ss}.
        /// </summary>
        public static string NormalTimeFormat {
            get {
                return ResourceManager.GetString("NormalTimeFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wartość nie jest liczbą.
        /// </summary>
        public static string NotANumberError {
            get {
                return ResourceManager.GetString("NotANumberError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pole jest puste lub zawiera tylko białe znaki.
        /// </summary>
        public static string NullOrEmptyError {
            get {
                return ResourceManager.GetString("NullOrEmptyError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Oryginalna wiadomość:.
        /// </summary>
        public static string OriginalMessage {
            get {
                return ResourceManager.GetString("OriginalMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Hasło.
        /// </summary>
        public static string PasswordName {
            get {
                return ResourceManager.GetString("PasswordName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Port.
        /// </summary>
        public static string PortName {
            get {
                return ResourceManager.GetString("PortName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Chip.
        /// </summary>
        public static string PunchChip {
            get {
                return ResourceManager.GetString("PunchChip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kod.
        /// </summary>
        public static string PunchCode {
            get {
                return ResourceManager.GetString("PunchCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Czas podbicia.
        /// </summary>
        public static string PunchTimestamp {
            get {
                return ResourceManager.GetString("PunchTimestamp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nazwa.
        /// </summary>
        public static string RelayName {
            get {
                return ResourceManager.GetString("RelayName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Czas kontroli.
        /// </summary>
        public static string ResultCheckTime {
            get {
                return ResourceManager.GetString("ResultCheckTime", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Chip.
        /// </summary>
        public static string ResultChip {
            get {
                return ResourceManager.GetString("ResultChip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Czas mety.
        /// </summary>
        public static string ResultFinishTime {
            get {
                return ResourceManager.GetString("ResultFinishTime", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Czas startu.
        /// </summary>
        public static string ResultStartTime {
            get {
                return ResourceManager.GetString("ResultStartTime", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nazwa.
        /// </summary>
        public static string RouteName {
            get {
                return ResourceManager.GetString("RouteName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kod.
        /// </summary>
        public static string RouteStepCode {
            get {
                return ResourceManager.GetString("RouteStepCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kolejność.
        /// </summary>
        public static string RouteStepOrder {
            get {
                return ResourceManager.GetString("RouteStepOrder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Schemat.
        /// </summary>
        public static string SchemaName {
            get {
                return ResourceManager.GetString("SchemaName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wyszukiwanie....
        /// </summary>
        public static string Searching {
            get {
                return ResourceManager.GetString("Searching", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Serwer.
        /// </summary>
        public static string ServerName {
            get {
                return ResourceManager.GetString("ServerName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Błąd stacji SPORTident.
        /// </summary>
        public static string SiHandlerError {
            get {
                return ResourceManager.GetString("SiHandlerError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Czas startu: {0:H:mm:ss}.
        /// </summary>
        public static string StartTimeFormat {
            get {
                return ResourceManager.GetString("StartTimeFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Czas rozpoczęcia jeszcze nie minął.
        ///Rozpocząć mimo to?.
        /// </summary>
        public static string StartTimeNotPassedWarning {
            get {
                return ResourceManager.GetString("StartTimeNotPassedWarning", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:H:mm:ss}.
        /// </summary>
        public static string TimestampFormat {
            get {
                return ResourceManager.GetString("TimestampFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Klasyfikacja.
        /// </summary>
        public static string TournamentClassification {
            get {
                return ResourceManager.GetString("TournamentClassification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kontynuuj.
        /// </summary>
        public static string TournamentContinue {
            get {
                return ResourceManager.GetString("TournamentContinue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Typ sztafety.
        /// </summary>
        public static string TournamentCourseType {
            get {
                return ResourceManager.GetString("TournamentCourseType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Nazwa.
        /// </summary>
        public static string TournamentName {
            get {
                return ResourceManager.GetString("TournamentName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Rozpocznij.
        /// </summary>
        public static string TournamentStart {
            get {
                return ResourceManager.GetString("TournamentStart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Czas rozpoczęcia.
        /// </summary>
        public static string TournamentStartTime {
            get {
                return ResourceManager.GetString("TournamentStartTime", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0:dd-M-yyyy H:mm:ss}.
        /// </summary>
        public static string TournamentStartTimeFormat {
            get {
                return ResourceManager.GetString("TournamentStartTimeFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Użytkownik.
        /// </summary>
        public static string UserName {
            get {
                return ResourceManager.GetString("UserName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wartość już istnieje.
        /// </summary>
        public static string ValueAlreadyExistsError {
            get {
                return ResourceManager.GetString("ValueAlreadyExistsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ostrzeżenie.
        /// </summary>
        public static string Warning {
            get {
                return ResourceManager.GetString("Warning", resourceCulture);
            }
        }
    }
}
