using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CityApp.Data.Enums;
using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using System.Security.Cryptography;

namespace CityApp.Data.Seeder
{
    public class CommonSeeder
    {

        public const long INITIAL_ACCOUNT_NUMBER = 75000;

        private readonly CommonContext _context;
        private IHostingEnvironment _hostingEnvironment;
        public IConfigurationRoot Configuration { get; set; }

        public string TextingDriving = "Texting & Driving";
        public string ServiceRequest = "Service Request";
        public string ParkingEnforcement = "Parking Enforcement";
        public string CodeEnforcement = "Code Enforcement";

        public string masterUserEmail = "info@govappsolutions.com";


        public CommonSeeder(CommonContext context, IHostingEnvironment environment)
        {
            _context = context;
            _hostingEnvironment = environment;
        }

        public void SeedEverythingForCommon()
        {
            SeedCities();
            SeedStates();
            SeedPartition();
            SeedAccountSettings();
            SeedViolation();
            SeedDefaultUsers();
            //SeedDefaultAccount();
        }

        private void SeedDefaultUsers()
        {
            if (!_context.Users.Any(m => m.Email == masterUserEmail))
            {
                var masterUser = new CommonUser();

                masterUser.Email = masterUserEmail;
                masterUser.SetPassword("Olympious911!");
                masterUser.Permission = SystemPermissions.Administrator;

                _context.Users.Add(masterUser);
                _context.SaveChanges();
            }
        }

        //Default account 
        private void SeedDefaultAccount()
        {
            var masterUser = _context.Users.Where(m => m.Email == masterUserEmail).Single();
            var masterCity = _context.Cities.Where(m => m.Name == "Sacramento").Single();
            var masterPartition = _context.Partitions.Where(m => m.Name == PARTITION_01_NAME).Single();

            var account = new CommonAccount {Name = "Master",
                OwnerUserId = masterUser.Id,
                CityId = masterCity.Id,
                Features = AccountFeatures.Info,
                Number = 50000,
                PartitionId = masterPartition.Id,
                State = "CA",
            };

            //TODO: Finish creating the rest of this. Bassically everything in CommonAccountService.CreateAccount


        }

        private void SeedViolation()
        {
            if (!_context.CommonViolations.Any())
            {
                SeedViolationTypes();
                SeedViolationCategories();

                var categories = _context.CommonViolationCategories.ToList();

                //Texging and Driving
                var textingAndDrivingCategory = categories.Where(m => m.Name == TextingDriving).Single();
                _context.CommonViolations.Add(new CommonViolation { CategoryId = textingAndDrivingCategory.Id, Name = TextingDriving, RequiredFields = ViolationRequiredFields.VehicleInformation|ViolationRequiredFields.Video});

                //Texging and Driving
                var parkingEnforcement = categories.Where(m => m.Name == ParkingEnforcement).Single();
                _context.CommonViolations.Add(new CommonViolation { CategoryId = parkingEnforcement.Id, RequiredFields = ViolationRequiredFields.VehicleInformation | ViolationRequiredFields.Video, Name = "Parking Without Permits", Description = "Report misuse of permit parking area, lack of displayed permit, or parking violation in permit only area." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = parkingEnforcement.Id, RequiredFields = ViolationRequiredFields.VehicleInformation | ViolationRequiredFields.Video, Name = "Handicap Parking", Description = "Report illegal use of a handicap parking spot, lack of handicap sign/license or obstruction of handicap parking zone." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = parkingEnforcement.Id, RequiredFields = ViolationRequiredFields.VehicleInformation | ViolationRequiredFields.Video, Name = "Fire Zone", Description = "Report vehicle parked in fire zone, fire lane, or blocking a hydrant.  Report any fire hydrant blockages (snow, ice, or downed trees)." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = parkingEnforcement.Id, RequiredFields = ViolationRequiredFields.VehicleInformation | ViolationRequiredFields.Video, Name = "Expired Meter", Description = "Report expired meters or unpaid meters. Report expired time in city parking ramp or city lot." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = parkingEnforcement.Id, RequiredFields = ViolationRequiredFields.VehicleInformation | ViolationRequiredFields.Video, Name = "Double Parked", Description = "Report double parked personal vehicle, commercial vehicle, or other hazard where double parking is illegal." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = parkingEnforcement.Id, RequiredFields = ViolationRequiredFields.VehicleInformation | ViolationRequiredFields.Video, Name = "Blocking Driveway", Description= "Report a vehicle illegally blocking a residential or commercial driveway.  Report blocked loading zones on commercial and private property." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = parkingEnforcement.Id, RequiredFields = ViolationRequiredFields.VehicleInformation | ViolationRequiredFields.Video, Name = "Blocking Crosswalk", Description = "Report parking violator blocking crosswalk or other pedestrian routes." });


                //Service Request
                var serviceRequestCategory = categories.Where(m => m.Name == ServiceRequest).Single();
                _context.CommonViolations.Add(new CommonViolation { CategoryId = serviceRequestCategory.Id, RequiredFields = ViolationRequiredFields.Video, Name = "Street light issue", Description = "Report a broken or burned out street light in a residential or commercial area.  Report malfunctioning a stoplight or burned out stoplight." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = serviceRequestCategory.Id, RequiredFields = ViolationRequiredFields.Video, Name = "Graffiti", Description = "Report graffiti, vandalism, and other non-sanctioned markings on city bridges, streets, retaining walls, buildings, and city managed property." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = serviceRequestCategory.Id, RequiredFields = ViolationRequiredFields.Video, Name = "Street Cleaning", Description = "Request street cleaning if hazardous chemicals are spilled, dead animals are present, or downed leaves/branches block drainage following a weather incident." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = serviceRequestCategory.Id, RequiredFields = ViolationRequiredFields.Video, Name = "Repair Pothole", Description = "Report pothole or concrete/cement issues on interstates, highways, public parking areas, and city/county maintained roads." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = serviceRequestCategory.Id, RequiredFields = ViolationRequiredFields.Video, Name = "Curb/sidewalk repair", Description = "Report missing pavement, raised/sunken/uneven pavement, holes cracks in the pavement or missing sewer vent covers." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = serviceRequestCategory.Id, RequiredFields = ViolationRequiredFields.Video, Name = "Dumping", Description = "Report illegal dumping in public areas or misuse of city dumpsters." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = serviceRequestCategory.Id, RequiredFields = ViolationRequiredFields.Video, Name = "Manhole Covers", Description = "Report missing manhole covers, raised manhole covers, or improper warning signs to work in progress around manholes." });


                //Service Request
                var codeEnforcementCategory = categories.Where(m => m.Name == CodeEnforcement).Single();
                _context.CommonViolations.Add(new CommonViolation { CategoryId = codeEnforcementCategory.Id, RequiredFields = ViolationRequiredFields.Video, Name = "Construction outside of hours", Description = "Report a broken or burned out street light in a residential or commercial area.  Report malfunctioning a stoplight or burned out stoplight." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = codeEnforcementCategory.Id, RequiredFields = ViolationRequiredFields.Video, Name = "Construction without Permit", Description = "Report a home remodel project that has not obtained the correct permits. " });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = codeEnforcementCategory.Id, RequiredFields = ViolationRequiredFields.Video, Name = "Unlicensed Business", Description = "Request street cleaning if hazardous chemicals are spilled, dead animals are present, or downed leaves/branches block drainage following a weather incident." });
                _context.CommonViolations.Add(new CommonViolation { CategoryId = codeEnforcementCategory.Id, RequiredFields = ViolationRequiredFields.Video, Name = "Unlicensed Street Vendor", Description = "Report pothole or concrete/cement issues on interstates, highways, public parking areas, and city/county maintained roads." });


                _context.SaveChanges();
            }
        }

        private void SeedViolationTypes()
        {
            _context.CommonViolationTypes.Add(new CommonViolationType { Name = TextingDriving});
            _context.CommonViolationTypes.Add(new CommonViolationType { Name = ServiceRequest });
            _context.CommonViolationTypes.Add(new CommonViolationType { Name = ParkingEnforcement });
            _context.CommonViolationTypes.Add(new CommonViolationType { Name = CodeEnforcement });

            _context.SaveChanges();
        }

        private void SeedViolationCategories()
        {
            var types = _context.CommonViolationTypes.ToList();

            //Texing and Driving
            var textDrivingServiceType = types.Where(m => m.Name == TextingDriving).Single();
            _context.CommonViolationCategories.Add(new CommonViolationCategory { TypeId = textDrivingServiceType.Id, Name = TextingDriving });

            //Parking Enforcement
            var parkingEnforcementType = types.Where(m => m.Name == ParkingEnforcement).Single();
            _context.CommonViolationCategories.Add(new CommonViolationCategory { TypeId = parkingEnforcementType.Id, Name = ParkingEnforcement });

            //Service Request
            var serviceRequestType = types.Where(m => m.Name == ServiceRequest).Single();
            _context.CommonViolationCategories.Add(new CommonViolationCategory { TypeId = serviceRequestType.Id, Name = ServiceRequest });

            //Code Enforcement 
            var codeEnforcementType = types.Where(m => m.Name == CodeEnforcement).Single();
            _context.CommonViolationCategories.Add(new CommonViolationCategory { TypeId = codeEnforcementType.Id, Name = CodeEnforcement });

            _context.SaveChanges();

        }

        /// <summary>
        /// Seeding city data
        /// </summary>
        private void SeedCities()
        {
            //Open another database context.  This could take a while


            if (_context.Cities.Any())
            {
                return;
            }


            // get data from csv file and insert into database
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, @"MigrateFile\us-cities.csv");
            var query = (from line in File.ReadLines(filePath)
                         let csvLines = line.Split(';')
                         from csvLine in csvLines
                         where !String.IsNullOrWhiteSpace(csvLine)
                         let data = csvLine.Split(',')
                         select new
                         {
                             Id = data[0],
                             Name = data[1],
                             County = data[2],
                             StateCode = data[3],
                             State = data[4],
                             Type = data[5],
                             Latitude = data[6],
                             Longitude = data[7]
                         }).ToList().ConvertAll(x => new City()
                         {
                             Name = x.Name,
                             County = x.County,
                             StateCode = x.StateCode,
                             State = x.State,
                             Type = x.Type,
                             Latitude =  Convert.ToDecimal(x.Latitude),
                             Longitude = Convert.ToDecimal(x.Longitude),
                             TimeZone = "Pacific Standard Time"
                         });

            //Only Insert Citites in California and Nevada
            query = query.Where(m => m.Type == "City" && (m.StateCode == "CA" || m.StateCode == "NV")).ToList();
            //Create 75 at a time, then take a break.  Sql Server has a tough time handling this .
            var batch = 75;
            var page = (query.Count / batch)+1;
            var cityIndex = 0;
            for(int i = 0; i < page; i++)
            {
                for (var index = 0; index < batch; index++)
                {
                    if (cityIndex < query.Count)
                    {
                        _context.Cities.Add(query[cityIndex]);
                        ++cityIndex;
                    }
                }
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// The values of these strings should match the connection strings in app config
        /// </summary>
        private const string PARTITION_01_NAME = "Accounts01";
        private const string PARTITION_02_NAME = "Accounts02";

        private void SeedPartition()
        {
            if (_context.Partitions.Any())
            {
                return;
            }


            //For test purposes, we'll add 2 partition. When going into production, we'll only want 1
            var partition = new Partition { Id = Guid.Parse("392020c6-31bf-4582-9cfc-0a441ca4795f"), Name = PARTITION_01_NAME };
            var partition2 = new Partition { Id = Guid.Parse("3877d974-ba9d-456a-985d-1b1792776cfc"), Name = PARTITION_02_NAME };

            partition.ConnectionString = Encrypt(Configuration[$"Data:{partition.Name}:ConnectionString"]);
            partition2.ConnectionString = Encrypt(Configuration[$"Data:{partition2.Name}:ConnectionString"]);
            _context.Partitions.Add(partition);
            _context.Partitions.Add(partition2);
            _context.SaveChanges();
        }

        private void SeedStates()
        {
            if (_context.States.Any())
            {
                return;
            }

            _context.States.Add(new State { Name = "Alabama" });
            _context.States.Add(new State { Name = "Alaska" });
            _context.States.Add(new State { Name = "Arizona" });
            _context.States.Add(new State { Name = "Arkansas" });
            _context.States.Add(new State { Name = "California" });
            _context.States.Add(new State { Name = "Colorado" });
            _context.States.Add(new State { Name = "Connecticut" });
            _context.States.Add(new State { Name = "Delaware" });
            _context.States.Add(new State { Name = "Florida" });
            _context.States.Add(new State { Name = "Georgia" });
            _context.States.Add(new State { Name = "Hawaii" });
            _context.States.Add(new State { Name = "Idaho" });
            _context.States.Add(new State { Name = "Illinois Indiana" });
            _context.States.Add(new State { Name = "Iowa" });
            _context.States.Add(new State { Name = "Kansas" });
            _context.States.Add(new State { Name = "Kentucky" });
            _context.States.Add(new State { Name = "Louisiana" });
            _context.States.Add(new State { Name = "Maine" });
            _context.States.Add(new State { Name = "Maryland" });
            _context.States.Add(new State { Name = "Massachusetts" });
            _context.States.Add(new State { Name = "Michigan" });
            _context.States.Add(new State { Name = "Minnesota" });
            _context.States.Add(new State { Name = "Mississippi" });
            _context.States.Add(new State { Name = "Missouri" });
            _context.States.Add(new State { Name = "Montana Nebraska" });
            _context.States.Add(new State { Name = "Nevada" });
            _context.States.Add(new State { Name = "New Hampshire" });
            _context.States.Add(new State { Name = "New Jersey" });
            _context.States.Add(new State { Name = "New Mexico" });
            _context.States.Add(new State { Name = "New York" });
            _context.States.Add(new State { Name = "North Carolina" });
            _context.States.Add(new State { Name = "North Dakota" });
            _context.States.Add(new State { Name = "Ohio" });
            _context.States.Add(new State { Name = "Oklahoma" });
            _context.States.Add(new State { Name = "Oregon" });
            _context.States.Add(new State { Name = "Pennsylvania Rhode Island" });
            _context.States.Add(new State { Name = "South Carolina" });
            _context.States.Add(new State { Name = "South Dakota" });
            _context.States.Add(new State { Name = "Tennessee" });
            _context.States.Add(new State { Name = "Texas" });
            _context.States.Add(new State { Name = "Utah" });
            _context.States.Add(new State { Name = "Vermont" });
            _context.States.Add(new State { Name = "Virginia" });
            _context.States.Add(new State { Name = "Washington" });
            _context.States.Add(new State { Name = "West Virginia" });
            _context.States.Add(new State { Name = "Wisconsin" });
            _context.SaveChanges();
        }

        private void SeedAccountSettings()
        {
            if (_context.CommonAccountSettings.Any())
            {
                return;
            }

            // All existing accounts will need one if they don't already have one.
            var commonAccounts = _context.CommonAccounts
                .Include(ca => ca.Settings)
                .ToArray();

            var accountsWithoutSettings = commonAccounts.Where(ca => ca.Settings == null).ToArray();

            foreach (var commonAccount in accountsWithoutSettings)
            {
                // Accept the defaults as defined on the object.
                commonAccount.Settings = new CommonAccountSettings { Id = commonAccount.Id };
            }

            _context.SaveChanges();
        }

        private CommonAccount CreateCommonAccount(Guid id, string name, long accountNumber, Guid ownerUserId, Partition partition, string fflNumber)
        {
            return new CommonAccount
            {
                Id = id,
                Name = name,
                Number = accountNumber,
                OwnerUserId = ownerUserId,
                Partition = partition
            };
        }

        private CommonUserAccount CreateCommonUserAccount(Guid accountId, Guid userId, AccountPermissions permissions)
        {
            return new CommonUserAccount
            {
                AccountId = accountId,
                UserId = userId,
                Permissions = permissions
            };
        }




        public void ApplyAccountMigrations()
        {
            var acctSeeder = new AccountSeeder();
            var partitions = _context.Partitions.ToArray();

            foreach (var partition in partitions)
            {
                // Apply migrations in each account partition.

                var builder = new DbContextOptionsBuilder<AccountContext>();
                builder.UseSqlServer(Decrypt(partition.ConnectionString));

                using (var db = new AccountContext(builder.Options))
                {
                    db.Database.Migrate();

                    //Get the accounts that use this partition
                    var partitionCommonAccounts = _context.CommonAccounts.Where(m => m.Partition.Id == partition.Id).ToArray();

                    //Seed account level data
                    //acctSeeder.SetupAccountAndUsers(_context, db, partitionCommonAccounts);
                }
            }
        }

        public static readonly string Key = "MAKV2SPBNI99212";
        private object user;

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = Key;
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = Key;
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
    }

}
