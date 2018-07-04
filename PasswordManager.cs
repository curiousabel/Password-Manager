/*
 * Program:         PasswordManager.exe
 * Module:          PasswordManager.cs
 * Date:            <2018-05-25>
 * Author:          <Abel>
 * Description:     Some free starting code for INFO-3110 project 1, the PasswordManager
 *                  application.
 *                  
 *                  Note that password strings can be tested for "strength" using the Password
 *                  class provided in this project.  Here's a quick example:
 *                  
 *                  Password pw = new Password("somepassword");
 *                  Console.WriteLine("That password is " + pw.StrengthLabel);
 *                  Console.WriteLine("That password has a strength of " + pw.StrengthPercent + "%");
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;


// Namespaces added manually
using System.Xml;           // XmlReader, XmlDocument and XmlReaderSetting classes
using System.Xml.Schema;    // XmlSchemaValidationFlags class
using System.IO;            // File class

namespace PasswordManager
{
    class Program
    {
        // Member variables
        private static string xmlFile = "";
        private static bool valid_xml = true;
        private static XmlDocument doc = null;

        static void Main(string[] args)
        {
            try
            {
                // Print a blank line
                Console.WriteLine();

                // Get the name of the XML file 
                if (args.Count() > 0 && File.Exists(args[0]))
                {
                    // Getting XML file name from the command line
                    xmlFile = args[0];
                }
                else
                {
                    // Ask the user to input the file name 
                    bool invalidFile = true;
                    do
                    {
                        Console.Write("Enter the path name of your passwords file: ");
                        xmlFile = Console.ReadLine();

                        if (!File.Exists(xmlFile))
                            Console.WriteLine("ERROR: The file '{0}' can't be found!", xmlFile);
                        else
                            invalidFile = false;

                    } while (invalidFile);

                    // Print a blank line
                    Console.WriteLine();
                }

                // Set the validation settings
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                ValidationEventHandler handler
                        = new ValidationEventHandler(ValidationCallback);
                settings.ValidationEventHandler += handler;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;

                // Create the XmlReader object and read/validate the XML file
                XmlReader reader = XmlReader.Create(xmlFile, settings);



                // Load the xml into the DOM
                doc = new XmlDocument();
                doc.Load(reader);

                if (valid_xml)
                {
                    reader.Close();
                    // ************************** ADD YOUR MAIN CODE HERE! **************************
                    
                    menu();

                }
            }
            catch (XmlException ex)
            {
                Console.WriteLine("{0,-20}{1}", "ERROR:", ex.Message);
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine("{0,-20}{1}", "ERROR:", ex.Message);
            }
            Console.WriteLine("Press any key to quit.");
            Console.ReadKey();
        } // end Main()


        // Create a menu to display the users input 
        private static void menu()
        {

            // display attris
            displaysUserName();
            // title bar 
            Console.WriteLine("+----------------------------------------------------------------------------------------------------------------------+");
            Console.WriteLine("|                                                 Password Enteries                                                    |");
            Console.WriteLine("|----------------------------------------------------------------------------------------------------------------------|");
            // This desplay the Description with the number 
            displayDesc();
            // Method to Ask the user what to do Next 
            displayCommand();
           // the Input value and store it 
            Console.Write("Enter a Command: ");
            string input = Console.ReadLine();

            // Depend on the values selected displays the proper output 
           
            if (input.ToUpper() == "A")
            {
                Console.WriteLine("Please Key-in values for the following fields...");
               
                Console.Write(" Description: ");
                string desc = Console.ReadLine();
                Console.Write(" User Id : ");
                string userid = Console.ReadLine();
                Console.Write(" Password: ");
                string password = Console.ReadLine();
                Console.Write(" Login Url: ");
                string loginUrl = Console.ReadLine();
                Console.Write(" Account Number: ");
                string accountnum = Console.ReadLine();
                addAccount(desc, userid, password, loginUrl, accountnum);
                menu();

            }
            else if (input.ToUpper() == "X")
            {
                
            }
            else
            {
                displayAccount(input);
            }

        }
        private static void displayCommand()
        {
            Console.WriteLine("************************************************************************************************************************");
            Console.WriteLine(" Press # from the above list to select an entry.");
            Console.WriteLine(" Press A to add a new entry");
            Console.WriteLine(" Press X to quit");

            Console.WriteLine("************************************************************************************************************************");

        }
        private static void displayChoices()
        {

          
            Console.WriteLine(" Press P to change this password");
            Console.WriteLine(" Press D to delete this entry");
            Console.WriteLine(" Press M to return to the main menu");

            Console.WriteLine("************************************************************************************************************************");

        }
        public static void addAccount(string desc, string userid, string password, string loginUrl, string accountnum)
        {
            XmlElement account = doc.CreateElement("account");
            account.SetAttribute("UserId", userid);
            account.SetAttribute("password", password);
            doc.DocumentElement.FirstChild.AppendChild(account);

            XmlElement description = doc.CreateElement("description");
            description.InnerText = desc;
            account.AppendChild(description);

            XmlElement AccountNumber = doc.CreateElement("account-Number");
            AccountNumber.InnerText = accountnum;
            account.AppendChild(AccountNumber);

            XmlElement date = doc.CreateElement("password-Date");
            date.InnerText = DateTime.Now.ToShortDateString();
            account.AppendChild(date);

            Password p = new Password(password);
            XmlElement Strength = doc.CreateElement("password-Strength");
            Strength.InnerText = p.StrengthLabel;
            account.AppendChild(Strength);
            XmlElement percentage = doc.CreateElement("password-percentage");
            percentage.InnerText = p.StrengthPercent.ToString();
            account.AppendChild(percentage);


            XmlElement LoginUrl = doc.CreateElement("Login-urI");
            LoginUrl.InnerText = desc;
            account.AppendChild(LoginUrl);

            doc.Save(xmlFile);
        }

        // deleting an account method 
        private static void deleteAccount(XmlNode selectedAccount)
        {
            selectedAccount.ParentNode.RemoveChild(selectedAccount);
            doc.Save(xmlFile);
            menu();
        }
        // deleting an Changing the password  method
        public static void ChangePassword(XmlNode selectedaccount)
        {
            // get the new password 
            Console.WriteLine("New Password: ");
            string newpassword = Console.ReadLine();
            // Get the strength and precentage tested 
            Password Changedpassword = new Password(newpassword);

            // Getting the Element and Attributes 
            XmlAttribute passwordAttribute = selectedaccount.Attributes["password"];
            XmlNode desc = selectedaccount.FirstChild;
            XmlNode accountNumber = desc.NextSibling;
            XmlNode PasswordDate = accountNumber.NextSibling;
            XmlNode PasswordStrength = PasswordDate.NextSibling;
            XmlNode PasswordPrecentage = PasswordStrength.NextSibling;

            // Setting the Attributes
            passwordAttribute.Value = Changedpassword.Value;
            PasswordDate.InnerText = DateTime.Now.ToShortDateString();
            PasswordStrength.InnerText = Changedpassword.StrengthLabel;
            PasswordPrecentage.InnerText = Changedpassword.StrengthPercent.ToString();
            doc.Save(xmlFile);
            menu();


        }

        private static void displayAccount(string number)
        {
            // Get the Parent Element 
            XmlNodeList AccountNodes = doc.GetElementsByTagName("account");
            // convert the string to int 
            int num = Convert.ToInt32(number);
            // Gett the Selected Account by the user 
            XmlNode selectAccount = AccountNodes.Item(num - 1);


            // Getting all it's child Elements and Attributes 
            XmlAttributeCollection attrAccount = selectAccount.Attributes;
            XmlNode desc = selectAccount.FirstChild;
            XmlNode accountNumber = desc.NextSibling;
            XmlNode PasswordDate = accountNumber.NextSibling;
            XmlNode PasswordStrength = PasswordDate.NextSibling;
            XmlNode PasswordPrecentage = PasswordStrength.NextSibling;
            XmlNode LoginUrl = PasswordPrecentage.NextSibling;

            // Display all the required fields to the console 

            Console.WriteLine("************************************************************************************************************************");
            Console.WriteLine("   " + number + "          " + desc.InnerText);

            Console.WriteLine("************************************************************************************************************************");
            Console.WriteLine("User ID:                   " + attrAccount.GetNamedItem("UserId").InnerText);
            Console.WriteLine("Password:                  " + attrAccount.GetNamedItem("password").InnerText);
            Console.WriteLine("Password Strength:         " + PasswordStrength.InnerText + "(" + PasswordPrecentage.InnerText + "%)");
            Console.WriteLine("Password Updated:          " + PasswordDate.InnerText);
            Console.WriteLine("Login url:                 " + LoginUrl.InnerText);
            Console.WriteLine("Account #:                 " + accountNumber.InnerText);

            Console.WriteLine("************************************************************************************************************************");
            // More Option the User can Select throught the program 
            displayChoices();
            Console.Write("Enter a Command:   ");
            string secondInput = Console.ReadLine();

           
            if (secondInput.ToUpper() == "P")
            {
                // Change the passwords 
                ChangePassword(selectAccount);
            }
            else if (secondInput.ToUpper() == "D")
            {
              // Deletes the Current Account selected 
                
                Console.Write("Delete ? {0}? (Y/N): ");
                string confirm = Console.ReadLine();
                if (confirm.ToUpper() == "Y")
                {
                    deleteAccount(selectAccount);
                }
                else
                {
                    // Return back to the menu 
                    menu();
                }


            }
            else if (secondInput.ToUpper() == "M")
            {
                // Back to the menu 
                menu();
            }

        }

        private static void displaysUserName()
        {
            
            XmlNodeList AccountNodes = doc.GetElementsByTagName("Username");
          
            foreach (XmlNode c in AccountNodes)
            {
                // get the name attribute for the account 
                //Note: XmlAttribute is an enchamnet of the xmlnamenodema 
                XmlAttributeCollection attrs = c.Attributes;
                XmlNode nameNode = attrs.GetNamedItem("name");
                // Display the name 
                if (nameNode == null)
                {
                    Console.WriteLine("Attribute Name not found");
                }
                else
                {
                    Console.WriteLine(" Password Management System for  " + nameNode.InnerText);
                }
            }
        }
        // Callback method to display validation errors and warnings if the xml file is invalid
        // according to its schema

        private static void displayDesc()
        {
            // get a nod list of all the accounts 
            XmlNodeList AccountNodes = doc.GetElementsByTagName("account");
            XmlNodeList desc = doc.GetElementsByTagName("description");
            for (int i = 0; i < AccountNodes.Count; i++)
            {
                XmlElement account = (XmlElement)AccountNodes[i];
                XmlElement descr = (XmlElement)desc[i];
                Console.WriteLine("     {0}.  {1} ", i + 1, descr.InnerText);
            }
            Console.WriteLine();
        }
        private static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.WriteLine("\n{0,-20}{1}", "WARNING:", args.Message);
            else
            {
                Console.WriteLine("\n{0,-20}{1}", "SCHEMA ERROR:", args.Message);
                valid_xml = false;
            }
        } // end ValidationCallback()

    } // end class

}
