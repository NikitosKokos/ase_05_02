public interface ISurveyResponseService
{
   bool CSVToDB();
   List<SurveyResponse> GetAllSurveyResponses();
   List<string> GetOccupationTypes();
   int GetTotalFeedbacksByType(int type);
   int GetAverageAge();
   bool ExportFamilyToCSV(int familySize, string fileName);
   bool ExportAIDataToDB();
}
public class SurveyResponseService : ISurveyResponseService
{
   private ISurveyResponseRepository _surveyResponseRepository;
   public SurveyResponseService(ISurveyResponseRepository surveyResponseRepository){
      _surveyResponseRepository = surveyResponseRepository;
   }

   public bool CSVToDB(){
      List<SurveyResponse> surveyResponseList = _surveyResponseRepository.GetSurveyResponsesCSV();

      return _surveyResponseRepository.ImportToDatabase(surveyResponseList);
   }

   public List<SurveyResponse> GetAllSurveyResponses(){
      return _surveyResponseRepository.GetAllSurveyResponses();
   }

   public List<string> GetOccupationTypes(){
      return _surveyResponseRepository.GetOccupationTypes();
   }
   public int GetTotalFeedbacksByType(int type){
      return _surveyResponseRepository.GetTotalFeedbacksByType(type);
   }
   public int GetAverageAge(){
      return _surveyResponseRepository.GetAverageAge();
   }

   public bool ExportFamilyToCSV(int familySize, string fileName){
      List<SurveyResponse> surveyResponseList = _surveyResponseRepository.GetFamilyResponces(familySize);

      return _surveyResponseRepository.ExportToCSV(fileName, surveyResponseList);
   }

   public bool ExportAIDataToDB(){
      List<AITrainData> aiTrainDataList = _surveyResponseRepository.GetAITrainData();
      return _surveyResponseRepository.ExportAIDataToDB(aiTrainDataList);
   }
}