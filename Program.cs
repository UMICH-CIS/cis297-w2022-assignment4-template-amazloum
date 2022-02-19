using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace PatientRecordApplication
{
    /// <Student>Ahmed Mazloum</Student>
    /// <Class>CIS297 Winter 2022</Class>
    /// <summary>
    /// This program allows user to store and retrive patients name and balance to and from a file
    /// </summary>
    
    class Program
    {
        
        static void Main(string[] args)
        {
            string fileName;
            Console.Write("Enter a valid file name: ");
            fileName = Console.ReadLine();
            PatientRecords inputRecords = new PatientRecords(fileName);

            Patient inputPatient = new Patient();
            int inputID = 0;
            string inputName;
            decimal inputBalance;

            while(inputID > -1)
            {
                Console.WriteLine("Entering a new Patient into the system. To end, input a negative number for ID.");
                inputID = checkValidID();
                if (inputID < 0)
                    break;
                Console.Write("Enter a Patient Name: ");
                inputName = Console.ReadLine();

                inputBalance = checkValidBalance();

                inputPatient.ID = inputID; 
                inputPatient.Name = inputName; 
                inputPatient.Balance = inputBalance;
                inputRecords.addPatient(inputPatient);
            }

            inputRecords.closeWriter();
            inputRecords.closeRecords();

            inputRecords.readAll();

            Console.Write("Enter a Patient ID to search for: ");
            int matchID = int.Parse(Console.ReadLine());
            inputRecords.readForPatientID(matchID);

            Console.Write("Enter a minimum balance to search for: ");
            decimal matchBalance = decimal.Parse(Console.ReadLine());
            inputRecords.readForGreaterBalance(matchBalance);
        }

       
        static int checkValidID()
        {
            int inputID;
            try
            {
                Console.Write("Enter a Patient ID number: ");
                inputID = int.Parse(Console.ReadLine());
            }
            catch(FormatException)
            {
                Console.Write("Please enter a valid integer!: ");
                inputID = int.Parse(Console.ReadLine());
            }
            return inputID;
        }

        
        static decimal checkValidBalance()
        {
            decimal inputBalance;
            try
            {
                Console.Write("Enter a Patient Balance: ");
                inputBalance = decimal.Parse(Console.ReadLine());
                if(inputBalance < 0)
                {
                    throw new InvalidBalanceException();
                }
            }
            catch(FormatException)
            {
                Console.Write("Please enter a valid decimal!: ");
                inputBalance = decimal.Parse(Console.ReadLine());
            }
            catch (InvalidBalanceException)
            {
                inputBalance = decimal.Parse(Console.ReadLine());
            }

            return inputBalance;
        }
    }
        /// <summary>
        /// Looks for a greater balance in file
        /// </summary>
        public void readForGreaterBalance(decimal matchBalance)
        {
            bool found = false;

            const char DELIM = ',';
            FileStream inFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);
            string patientIn;
            string[] fields;
            patientIn = reader.ReadLine();
            Patient outputPatient = new Patient();
            while (patientIn != null)
            {
                fields = patientIn.Split(DELIM);
                outputPatient.ID = int.Parse(fields[0]);
                outputPatient.Name = fields[1];
                outputPatient.Balance = decimal.Parse(fields[2]);
                if (outputPatient.Balance >= matchBalance)
                {
                    found = true;
                    Console.WriteLine("{0,-5}{1,-12}{2,8}", outputPatient.ID, outputPatient.Name, outputPatient.Balance.ToString("C"));
                }
                patientIn = reader.ReadLine();
            }

            if (!found)
                Console.WriteLine("No patients found with lower balance!");

            reader.Close();
            inFile.Close();
        }
    }
    class Patient
    {
      
        public int ID { get; set; }
        
        
        public string Name { get; set; }

  
        public decimal Balance { get; set; }
    }
     /// <summary>
    /// Holds the patient file so we can write and read from.
    /// </summary>
    class PatientRecords
    {
        private FileStream fstream;
        private StreamWriter writer;
        private string fileName;

       
        public PatientRecords(string inputFileName)
        {
            fstream = new FileStream(inputFileName, FileMode.Create, FileAccess.Write);
            writer = new StreamWriter(fstream);
            fileName = inputFileName;
        }

   
        public void closeRecords()
        {
            fstream.Close();
        }

      
        public void closeWriter()
        {
            writer.Close();
        }

       
        public void addPatient(Patient inputPatient)
        {
            const string DELIM = ",";
            writer.WriteLine(inputPatient.ID + DELIM + inputPatient.Name + DELIM + inputPatient.Balance);
        }

     
        public void readAll()
        {
            const char DELIM = ',';
            FileStream inFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);
            string patientIn;
            string[] fields;
            patientIn = reader.ReadLine();
            Patient outputPatient = new Patient();
            Console.WriteLine("\n{0,-5}{1,-12}{2,8}\n",
              "ID", "Name", "Balance");

       
            try
            {
                while (patientIn != null)
                {
                    fields = patientIn.Split(DELIM);
                    outputPatient.ID = int.Parse(fields[0]);
                    outputPatient.Name = fields[1];
                    outputPatient.Balance = decimal.Parse(fields[2]);
                    Console.WriteLine("{0,-5}{1,-12}{2,8}", outputPatient.ID, outputPatient.Name, outputPatient.Balance.ToString("C"));
                    patientIn = reader.ReadLine();
                }
            }
            catch
            {
                Console.WriteLine("Invalid Read, Data corrputed");
            }
            finally
            {
                reader.Close();
                inFile.Close();
            }
            
        }

    
        public void readForPatientID(int matchID)
        {
            bool found = false;

            const char DELIM = ',';
            FileStream inFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);
            string patientIn;
            string[] fields;
            patientIn = reader.ReadLine();
            Patient outputPatient = new Patient();

            while (patientIn != null)
            {
                fields = patientIn.Split(DELIM);
                outputPatient.ID = int.Parse(fields[0]);
                outputPatient.Name = fields[1];
                outputPatient.Balance = decimal.Parse(fields[2]);
                if(outputPatient.ID == matchID)
                {
                    found = true;
                    Console.WriteLine("{0,-5}{1,-12}{2,8}", outputPatient.ID, outputPatient.Name, outputPatient.Balance.ToString("C"));
                    break;
                }
                patientIn = reader.ReadLine();
            }

            if (!found)
                Console.WriteLine("Patient not found!");

            reader.Close();
            inFile.Close();
        }

       
    /// <summary>
    /// for a negative balance.
    /// </summary>
    class InvalidBalanceException :Exception
    {
        public InvalidBalanceException()
        {
            Console.Write("Please enter a non-negative balance!: ");
        }
    }
}




