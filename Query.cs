using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;
        

        public delegate void EmployeeOperator(Employee employee);

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
       
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            //Created Method To Allow For All Employee CRUD Ops To Occur.
            crudOperation = crudOperation.ToLower();
            switch (crudOperation)
            {
                case "create":
                    AddEmployee(employee);
                    break;
                case "remove":
                    RemoveEmployee(employee);
                    break;
                case "update":
                    UpdateEmployee(employee);
                    break;
                case "display":
                    EmployeeDisplayInfo(employee.EmployeeNumber);
                    break;
                default:
                    AddEmployee(employee);
                    break;
            }
        }
        
        internal static void AddEmployee(Employee employee)
        {
            db.Employees.InsertOnSubmit(employee);
            db.SubmitChanges();
        }

        internal static void RemoveEmployee(Employee employee)
        {
            db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).SingleOrDefault();
            db.SubmitChanges();
        }
        
        internal static void UpdateEmployee(Employee employee)
        {
            var update = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).SingleOrDefault();
            update.FirstName = employee.FirstName;
            update.LastName = employee.LastName;
            update.Email = employee.Email;
            db.SubmitChanges();
        }

        internal static void EmployeeDisplayInfo(int? employeeNumber)
        { 
            UserInterface.DisplayEmployee(db.Employees.Where(e => e.EmployeeNumber == employeeNumber).FirstOrDefault());
        }


        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {

            db = new HumaneSocietyDataContext();
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {

            var animal = db.Animals.Where(a => a.AnimalId == id).SingleOrDefault();
            UserInterface.DisplayAnimalInfo(animal);
            return animal;

        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {

            var update = db.Animals.Where(a => a.AnimalId == animalId).SingleOrDefault();
            foreach(KeyValuePair<int, string> key in updates)
            {
                switch (key.Key)
                {
                    case 1:
                        update.Category.Name = key.Value;
                        break;
                    case 2:
                        update.Name = key.Value;
                        break;
                    case 3:
                        update.Age = Convert.ToInt32(key.Value);
                        break;
                    case 4:
                        update.Demeanor = key.Value;
                        break;
                    case 5:
                        update.KidFriendly = key.Value == "true" ? true : false;
                        break;
                    case 6:
                        update.PetFriendly = key.Value == "true" ? true : false;
                        break;
                    case 7:
                        update.Weight = Convert.ToInt32(key.Value);
                        break;
                    case 8:
                        update.AnimalId = Convert.ToInt32(key.Value);
                        break;
                    default:
                        break;
                }
            }

        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(db.Animals.Where(x => x.AnimalId == animal.AnimalId).SingleOrDefault());
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> animal = db.Animals;
            foreach(KeyValuePair<int, string> key in updates)
            {
                switch (key.Key)
                {
                    case 1:
                        animal = animal.Where(a => a.AnimalId == Convert.ToInt32(key.Value));
                        break;
                    case 2:
                        animal = animal.Where(a => a.Name == key.Value);
                        break;
                    case 3:
                        animal = animal.Where(a => a.Age == Convert.ToInt32(key.Value));
                        break;
                    case 4:
                        animal = animal.Where(a => a.Demeanor == key.Value);
                        break;
                    case 5:
                        animal = animal.Where(a => a.KidFriendly.ToString() == key.Value);
                        break;
                    case 6:
                        animal = animal.Where(a => a.PetFriendly.ToString() == key.Value);
                        break;
                    case 7:
                        animal = animal.Where(a => a.Gender == key.Value);
                        break;
                    case 8:
                        animal = animal.Where(a => a.Weight == Convert.ToInt32(key.Value));
                        break;
                    case 9:
                        animal = animal.Where(a => a.Category.Name == key.Value);
                        break;
                    default:
                        break;
                }
            }
            return animal;
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            var category = db.Categories.Where(a => a.Name == categoryName).FirstOrDefault();
            int categoryid = category.CategoryId;
            return categoryid;
        }
        
        internal static Room GetRoom(int animalId)
        {
            var room = db.Rooms.Where(a => a.AnimalId == animalId).FirstOrDefault();
            return room;
        }
    

        internal static int GetDietPlanId(string dietPlanName)
        {
            var diet = db.DietPlans.Where(d => d.Name == dietPlanName).SingleOrDefault();
            int dietID = diet.DietPlanId;
            return dietID;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            throw new NotImplementedException();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var shots = db.AnimalShots.Where(a => a.Animal == animal);
            return shots;
        }

        public static Shot GetShot()
        {
            Shot shot = new Shot();
            var shot1 = db.Shots.Where(s => s == shot).First();
            return shot;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            Shot shot = new Shot();
            shot.Name = shotName;
            db.Shots.InsertOnSubmit(shot);
            AnimalShot animalshot = new AnimalShot();

            animalshot.DateReceived = DateTime.Now;
            animalshot.AnimalId = animal.AnimalId;
            animalshot.ShotId = shot.ShotId;
            db.AnimalShots.InsertOnSubmit(animalshot);

            db.SubmitChanges();
        }
    }
  

}