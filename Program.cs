ISurveyResponseRepository surveyResponseRepository= new SurveyResponseRepository();
ISurveyResponseService surveyResponseService = new SurveyResponseService(surveyResponseRepository);

bool isSelected = false;
while(!isSelected){
   Console.Clear();
   Console.WriteLine("\u001B[1mAI Train Data App\u001b[0m");
   Console.WriteLine("1 - Import CSV into database");
   Console.WriteLine("2 - View All Survey Response from database");
   Console.WriteLine("3 - Count total feedbacks based on feedback type (ask feedback type from user)");
   Console.WriteLine("4 - Average age of all survey responses");
   Console.WriteLine("5 - Show all occupation types");
   Console.WriteLine("6 - Ask for a family size and export the records to a CSV file. Ask the user for the csv file name");
   Console.WriteLine("7 - Export all survey responses to AI train data format in the database");
   Console.WriteLine("8 - Exit");
   int option = int.Parse(Console.ReadLine());
   ConsoleKeyInfo key;
   switch (option)
   {
      case 1:
         Console.Clear();
         bool isImported = surveyResponseService.CSVToDB();

         if(isImported){
            Console.WriteLine("\x1b[32mImported Successfully!\x1b[0m");
         }else{
            Console.WriteLine("\x1b[31mImported NOT Successfully!\x1b[0m");
         }
         Task.Delay(1600).Wait();
         break;
      case 2:
         Console.Clear();
         var surveyResponses = surveyResponseService.GetAllSurveyResponses();

         foreach (var surveyResponse in surveyResponses)
         {
            Console.WriteLine(surveyResponse);
         }

         Console.WriteLine("\nPress \x1b[4m\x1b[36mEnter\x1b[0m to go to the main menu");
         key = Console.ReadKey(true);

         switch (key.Key)
         {
            case ConsoleKey.Enter:
               break;
         }
         break;
      case 3:
         Console.Clear();
         Console.WriteLine("Enter feedback type(p - positive/n - negative)");
         string feedbackType = Console.ReadLine();
         if(feedbackType == "p" || feedbackType == "n"){
            int type = feedbackType == "p" ? 1 : 0;
            int totalFeedbacks = surveyResponseService.GetTotalFeedbacksByType(type);
            string typeStr = type == 1 ? "Positive" : "Negative";
            if(totalFeedbacks == -1){
               Console.WriteLine("\x1b[31mCouldn't read from db!\x1b[0m");
            }
            Console.WriteLine($"Total Feedbacks by type {typeStr} = \x1b[36m{totalFeedbacks}\x1b[0m");
         }else{
            Console.WriteLine("\x1b[31mNo such feedback type!\x1b[0m");
         }
         Task.Delay(1600).Wait();
         break;
      case 4:
         Console.Clear();

         int avgAge = surveyResponseService.GetAverageAge();

         Console.WriteLine($"Average age = \x1b[36m{avgAge}\x1b[0m");

         Console.WriteLine("\nPress \x1b[4m\x1b[36mEnter\x1b[0m to go to the main menu");
         key = Console.ReadKey(true);

         switch (key.Key)
         {
            case ConsoleKey.Enter:
               break;
         }
         break;
      case 5:
         Console.Clear();
         List<string> occupationTypes = surveyResponseService.GetOccupationTypes();

         Console.WriteLine("\u001B[1mOccupation Types\u001b[0m");

         foreach (var occupationType in occupationTypes){
            Console.WriteLine(occupationType);
         }

         Console.WriteLine("\nPress \x1b[4m\x1b[36mEnter\x1b[0m to go to the main menu");
         key = Console.ReadKey(true);

         switch (key.Key)
         {
            case ConsoleKey.Enter:
               break;
         }
         break;
      case 6:
         Console.Clear();
         Console.WriteLine("Enter a family size(from 1 to 6)");
         int familySize = int.Parse(Console.ReadLine());
         Console.WriteLine("Enter a file name");
         string fileName = Console.ReadLine();

         if(familySize < 1 || familySize > 6){
            Console.WriteLine("\x1b[31mNo such Family Size!\x1b[0m");
            Task.Delay(1600).Wait();
            break;
         }

         bool isExported = surveyResponseService.ExportFamilyToCSV(familySize, fileName);

         if(isExported){
            Console.WriteLine("\x1b[32mImported Successfully!\x1b[0m");
         }else{
            Console.WriteLine("\x1b[31mImported NOT Successfully!\x1b[0m");
         }

         Task.Delay(1600).Wait();
         break;
      case 7:
         Console.Clear();

         bool isSuccessful = surveyResponseService.ExportAIDataToDB();

         if(isSuccessful){
            Console.WriteLine("\x1b[32mImported Successfully!\x1b[0m");
         }else{
            Console.WriteLine("\x1b[31mImported NOT Successfully!\x1b[0m");
         }
         Task.Delay(1600).Wait();
         break;
      case 8:
         isSelected = true;
         Console.Clear();
         Environment.Exit(0);
         break;
      default:
         break;
   }
}
