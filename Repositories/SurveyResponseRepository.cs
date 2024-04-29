using MySql.Data.MySqlClient;

public interface ISurveyResponseRepository
{
   List<SurveyResponse> GetSurveyResponsesCSV();
   bool ImportToDatabase(List<SurveyResponse> surveyResponses);
   List<SurveyResponse> GetAllSurveyResponses();
   List<string> GetOccupationTypes();
   int GetTotalFeedbacksByType(int type);
   int GetAverageAge();
   bool ExportToCSV(string fileName, List<SurveyResponse> surveyResponses);
   List<SurveyResponse> GetFamilyResponces(int familySize);
   List<AITrainData> GetAITrainData();
   bool ExportAIDataToDB(List<AITrainData> aiTrainDataList);
}

public class SurveyResponseRepository : ISurveyResponseRepository
{
   private string _path = "./Files/onlinefoods.csv";
   private string _savePath = "./FamilyFiles";

      private SurveyResponse ParseLine(string line){
      var cols = line.Split(',');
      SurveyResponse surveyResponse = new SurveyResponse();
      surveyResponse.Age = int.Parse(cols[0]);
      surveyResponse.Gender = cols[1];
      surveyResponse.MaritalStatus = cols[2];
      surveyResponse.Occupation = cols[3];
      surveyResponse.MonthlyIncome = cols[4];
      surveyResponse.EducationalQualifications = cols[5];
      surveyResponse.FamilySize = int.Parse(cols[6]);
      surveyResponse.Latitude = double.Parse(cols[7]);
      surveyResponse.Longitude = double.Parse(cols[8]);
      surveyResponse.PinCode = int.Parse(cols[9]);
      surveyResponse.Output = cols[10] == "Yes" ? true : false;
      surveyResponse.Feedback = cols[11];
      
      return surveyResponse;
   }

   public List<SurveyResponse> GetSurveyResponsesCSV(){
      List<SurveyResponse> surveyResponses = new List<SurveyResponse>();
      try
      {
         string[] lines = File.ReadAllLines(_path);

         // skip first line
         lines = lines.Skip(1).ToArray();

         foreach(var line in lines){
            if(line.Trim().Length == 0){
               continue;
            }
            SurveyResponse surveyResponse = ParseLine(line);
            surveyResponses.Add(surveyResponse);
         }
      }
      catch (Exception ex)
      {
         Console.WriteLine("Error reading from file: " + ex.Message);
         return null;
      }
      return surveyResponses;
   }

   public List<SurveyResponse> GetFamilyResponces(int familySize){
      List<SurveyResponse> surveyResponses = new List<SurveyResponse>();
      try{
         
         MySqlConnection mySqlConnection = CreateConnection();
         mySqlConnection.Open();
         MySqlCommand command = new MySqlCommand("SELECT * FROM SurveyResponses WHERE FamilySize = @FamilySize", mySqlConnection);
         command.Parameters.AddWithValue("@FamilySize", familySize);

         MySqlDataReader reader = command.ExecuteReader();
         while (reader.Read()){
            SurveyResponse newSurveyResponse = new SurveyResponse();
            newSurveyResponse.Id = reader.GetInt32("Id");
            newSurveyResponse.Age = reader.GetInt32("Age");
            newSurveyResponse.Gender = reader.GetString("Gender");
            newSurveyResponse.MaritalStatus = reader.GetString("MaritalStatus");
            newSurveyResponse.Occupation = reader.GetString("Occupation");
            newSurveyResponse.MonthlyIncome = reader.GetString("MonthlyIncome");
            newSurveyResponse.EducationalQualifications = reader.GetString("EducationalQualifications");
            newSurveyResponse.FamilySize = reader.GetInt32("FamilySize");
            newSurveyResponse.Latitude = reader.GetDouble("Latitude");
            newSurveyResponse.Longitude = reader.GetDouble("Longitude");
            newSurveyResponse.PinCode = reader.GetInt32("PinCode");
            newSurveyResponse.Output = reader.GetBoolean("Output");
            newSurveyResponse.Feedback = reader.GetString("Feedback");

            surveyResponses.Add(newSurveyResponse);
         }
         mySqlConnection.Close();

      }catch(Exception ex){
         Console.WriteLine(ex);
      }

      return surveyResponses;
   }

   public List<AITrainData> GetAITrainData(){
      List<AITrainData> aiTrainDataList = new List<AITrainData>();
      try{
         
         MySqlConnection mySqlConnection = CreateConnection();
         mySqlConnection.Open();
         MySqlCommand command = new MySqlCommand("SELECT * FROM SurveyResponses", mySqlConnection);

         MySqlDataReader reader = command.ExecuteReader();
         while (reader.Read()){
            AITrainData aiTrainData = new AITrainData();
            aiTrainData.Age = reader.GetInt32("Age");
            aiTrainData.Gender = reader.GetString("Gender");
            aiTrainData.MaritalStatus = reader.GetString("MaritalStatus");
            aiTrainData.Occupation = reader.GetString("Occupation");
            aiTrainData.MonthlyIncome = reader.GetString("MonthlyIncome");
            aiTrainData.FamilySize = reader.GetInt32("FamilySize");

            aiTrainDataList.Add(aiTrainData);
         }
         mySqlConnection.Close();

      }catch(Exception ex){
         Console.WriteLine(ex);
      }

      return aiTrainDataList;
   }

   public bool ExportAIDataToDB(List<AITrainData> aiTrainDataList){
      try{
         // create a new table AITrainData
         initTable("AITrainData");

         MySqlConnection mySqlConnection = CreateConnection();
         mySqlConnection.Open();

         foreach(var aiTrainData in aiTrainDataList){
            MySqlCommand mySqlCommand = new MySqlCommand(
                @"INSERT INTO AITrainData 
                    (Age, Gender, MaritalStatus, Occupation, MonthlyIncome, FamilySize) 
                VALUES 
                    (@Age, @Gender, @MaritalStatus, @Occupation, @MonthlyIncome, @FamilySize);", mySqlConnection);

            // Add parameters
            mySqlCommand.Parameters.AddWithValue("@Age", aiTrainData.Age);
            mySqlCommand.Parameters.AddWithValue("@Gender", aiTrainData.Gender);
            mySqlCommand.Parameters.AddWithValue("@MaritalStatus", aiTrainData.MaritalStatus);
            mySqlCommand.Parameters.AddWithValue("@Occupation", aiTrainData.Occupation);
            mySqlCommand.Parameters.AddWithValue("@MonthlyIncome", aiTrainData.MonthlyIncome);
            mySqlCommand.Parameters.AddWithValue("@FamilySize", aiTrainData.FamilySize);

            // Execute the query
            mySqlCommand.ExecuteNonQuery();
         }
         
         return true;
      }catch(Exception ex){
         Console.WriteLine(ex);
         return false;
      }
   }

   public bool ExportToCSV(string fileName, List<SurveyResponse> surveyResponses){
      try{
         string filePath = $"{Path.Combine(_savePath, fileName)}.csv";

         using (StreamWriter writer = new StreamWriter(filePath))
         {
            writer.WriteLine("Age,Gender,Marital Status,Occupation,Monthly Income,Educational Qualifications,Family size,latitude,longitude,Pin code,Output,Feedback,");
            foreach(var surveyResponse in surveyResponses){
               string line = $"{surveyResponse.Age},{surveyResponse.Gender},{surveyResponse.MaritalStatus},{surveyResponse.Occupation},{surveyResponse.MonthlyIncome},{surveyResponse.EducationalQualifications},{surveyResponse.FamilySize},{surveyResponse.Latitude},{surveyResponse.Longitude},{surveyResponse.PinCode},{surveyResponse.Output},{surveyResponse.Feedback}";
               writer.WriteLine(line);
            }
         }
         return true;
      }catch(Exception ex){
         Console.WriteLine(ex);
         return false;
      }
   }

   public bool ImportToDatabase(List<SurveyResponse> surveyResponses){
      try{
         // create a new table SurveyResponses
         initTable("SurveyResponses");
         MySqlConnection mySqlConnection = CreateConnection();
         mySqlConnection.Open();

         foreach(var surveyResponse in surveyResponses){
            MySqlCommand mySqlCommand = new MySqlCommand(
                @"INSERT INTO SurveyResponses 
                    (Age, Gender, MaritalStatus, Occupation, MonthlyIncome, EducationalQualifications, FamilySize, Latitude, Longitude, PinCode, Output, Feedback) 
                VALUES 
                    (@Age, @Gender, @MaritalStatus, @Occupation, @MonthlyIncome, @EducationalQualifications, @FamilySize, @Latitude, @Longitude, @PinCode, @Output, @Feedback);", mySqlConnection);

            // Add parameters
            mySqlCommand.Parameters.AddWithValue("@Age", surveyResponse.Age);
            mySqlCommand.Parameters.AddWithValue("@Gender", surveyResponse.Gender);
            mySqlCommand.Parameters.AddWithValue("@MaritalStatus", surveyResponse.MaritalStatus);
            mySqlCommand.Parameters.AddWithValue("@Occupation", surveyResponse.Occupation);
            mySqlCommand.Parameters.AddWithValue("@MonthlyIncome", surveyResponse.MonthlyIncome);
            mySqlCommand.Parameters.AddWithValue("@EducationalQualifications", surveyResponse.EducationalQualifications);
            mySqlCommand.Parameters.AddWithValue("@FamilySize", surveyResponse.FamilySize);
            mySqlCommand.Parameters.AddWithValue("@Latitude", surveyResponse.Latitude);
            mySqlCommand.Parameters.AddWithValue("@Longitude", surveyResponse.Longitude);
            mySqlCommand.Parameters.AddWithValue("@PinCode", surveyResponse.PinCode);
            mySqlCommand.Parameters.AddWithValue("@Output", surveyResponse.Output ? 1 : 0);
            mySqlCommand.Parameters.AddWithValue("@Feedback", surveyResponse.Feedback);

            // Execute the query
            mySqlCommand.ExecuteNonQuery();
         }
         
         return true;
      }catch(Exception ex){
         Console.WriteLine(ex);
         return false;
      }
   }
   public List<string> GetOccupationTypes(){
      List<string> occupationTypes = new List<string>();
      try{
         
         MySqlConnection mySqlConnection = CreateConnection();
         mySqlConnection.Open();
         MySqlCommand command = new MySqlCommand("SELECT DISTINCT Occupation FROM SurveyResponses", mySqlConnection);

         MySqlDataReader reader = command.ExecuteReader();
         while (reader.Read()){
            string occupation = reader.GetString("Occupation");
            occupationTypes.Add(occupation);
         }
         mySqlConnection.Close();

      }catch(Exception ex){
         Console.WriteLine(ex);
      }

      return occupationTypes;
   }
   public List<SurveyResponse> GetAllSurveyResponses(){
      List<SurveyResponse> surveyResponses = new List<SurveyResponse>();
      try{
         
         MySqlConnection mySqlConnection = CreateConnection();
         mySqlConnection.Open();
         MySqlCommand command = new MySqlCommand("SELECT * FROM SurveyResponses", mySqlConnection);

         MySqlDataReader reader = command.ExecuteReader();
         while (reader.Read()){
            SurveyResponse newSurveyResponse = new SurveyResponse();
            newSurveyResponse.Id = reader.GetInt32("Id");
            newSurveyResponse.Age = reader.GetInt32("Age");
            newSurveyResponse.Gender = reader.GetString("Gender");
            newSurveyResponse.MaritalStatus = reader.GetString("MaritalStatus");
            newSurveyResponse.Occupation = reader.GetString("Occupation");
            newSurveyResponse.MonthlyIncome = reader.GetString("MonthlyIncome");
            newSurveyResponse.EducationalQualifications = reader.GetString("EducationalQualifications");
            newSurveyResponse.FamilySize = reader.GetInt32("FamilySize");
            newSurveyResponse.Latitude = reader.GetDouble("Latitude");
            newSurveyResponse.Longitude = reader.GetDouble("Longitude");
            newSurveyResponse.PinCode = reader.GetInt32("PinCode");
            newSurveyResponse.Output = reader.GetBoolean("Output");
            newSurveyResponse.Feedback = reader.GetString("Feedback");

            surveyResponses.Add(newSurveyResponse);
         }
         mySqlConnection.Close();

      }catch(Exception ex){
         Console.WriteLine(ex);
      }

      return surveyResponses;
   }
   public int GetTotalFeedbacksByType(int type){
      try{
         int totalFeedbacks = 0;
         MySqlConnection mySqlConnection = CreateConnection();
         mySqlConnection.Open();
         MySqlCommand command = new MySqlCommand("SELECT count(Feedback) as 'Feedback' FROM SurveyResponses WHERE Feedback = @Feedback", mySqlConnection);
         command.Parameters.AddWithValue("@Feedback", type == 1 ? "Positive": "Negative ");

         MySqlDataReader reader = command.ExecuteReader();
         if(reader.Read()){
            totalFeedbacks = reader.GetInt32("Feedback");
         }
         mySqlConnection.Close();

         return totalFeedbacks;
      }catch(Exception ex){
         Console.WriteLine(ex);
         return -1;
      }
   }
   public int GetAverageAge(){
      try{
         int avgAge = 0;
         MySqlConnection mySqlConnection = CreateConnection();
         mySqlConnection.Open();
         MySqlCommand command = new MySqlCommand("SELECT round(avg(Age)) as 'Age' FROM SurveyResponses", mySqlConnection);

         MySqlDataReader reader = command.ExecuteReader();
         if(reader.Read()){
            avgAge = reader.GetInt32("Age");
         }
         mySqlConnection.Close();

         return avgAge;
      }catch(Exception ex){
         Console.WriteLine(ex);
         return -1;
      }
   }

   private bool initTable(string tableName){
      try{
         
         using(MySqlConnection mySqlConnection = CreateConnection()){
            mySqlConnection.Open();
            MySqlCommand mySqlCommand = new MySqlCommand(
                  $"DROP TABLE IF EXISTS {tableName};", mySqlConnection);

            // Execute the query
            mySqlCommand.ExecuteNonQuery();
         }

         switch (tableName)
         {
            case "AITrainData":
               using(MySqlConnection mySqlConnection = CreateConnection()){
                  mySqlConnection.Open();
                  MySqlCommand mySqlCommand = new MySqlCommand(
                        "CREATE TABLE AITrainData (ID INT AUTO_INCREMENT PRIMARY KEY,Age INT,Gender VARCHAR(255),MaritalStatus VARCHAR(255),Occupation VARCHAR(255),MonthlyIncome VARCHAR(255),FamilySize INT);", mySqlConnection);

                  // Execute the query
                  mySqlCommand.ExecuteNonQuery();
               }
               return true;
            case "SurveyResponses":
               using(MySqlConnection mySqlConnection = CreateConnection()){
                  mySqlConnection.Open();
                  MySqlCommand mySqlCommand = new MySqlCommand(
                        "CREATE TABLE SurveyResponses (Id INT AUTO_INCREMENT PRIMARY KEY,Age INT,Gender VARCHAR(50),MaritalStatus VARCHAR(50),Occupation VARCHAR(100),MonthlyIncome VARCHAR(100),EducationalQualifications VARCHAR(100),FamilySize INT,Latitude DOUBLE,Longitude DOUBLE,PinCode VARCHAR(20),Output BOOLEAN,Feedback VARCHAR(255));", mySqlConnection);

                  // Execute the query
                  mySqlCommand.ExecuteNonQuery();
               }
               return true;
            default:
               return false;
         }
      }catch(Exception ex){
         Console.WriteLine(ex);
         return false;
      }
   }
   private MySqlConnection CreateConnection(){
      string connectionString = "server=localhost;database=Ex02;user=root;pwd=root";
      MySqlConnection connection = new MySqlConnection(connectionString);
      return connection;
   }
}