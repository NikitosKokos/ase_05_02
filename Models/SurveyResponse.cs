public class SurveyResponse
{
   public int Id { get; set; }
   public int Age { get; set; }
   public string Gender { get; set; }
   public string MaritalStatus { get; set; }
   public string Occupation { get; set; }
   public string MonthlyIncome { get; set; }
   public string EducationalQualifications { get; set; }
   public int FamilySize { get; set; }
   public double Latitude { get; set; }
   public double Longitude { get; set; }
   public int PinCode { get; set; }
   public bool Output { get; set; }
   public string Feedback { get; set; }

   public override string ToString()
   {
      return $"\n\x1b[36mId:\x1b[0m {Id}, \x1b[36mAge:\x1b[0m {Age}, \x1b[36mGender:\x1b[0m {Gender}, \x1b[36mMarital Status:\x1b[0m {MaritalStatus}, \x1b[36mOccupation:\x1b[0m {Occupation}, \x1b[36mMonthly Income:\x1b[0m {MonthlyIncome}, \x1b[36mEducational Qualifications:\x1b[0m {EducationalQualifications}, \x1b[36mFamily Size:\x1b[0m {FamilySize}, \x1b[36mLatitude:\x1b[0m {Latitude}, \x1b[36mLongitude:\x1b[0m {Longitude}, \x1b[36mPin Code:\x1b[0m {PinCode}, \x1b[36mOutput:\x1b[0m {(Output ? "True" : "False")}, \x1b[36mFeedback:\x1b[0m {Feedback}";
   }
}