using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using System.Web.Helpers;
using System.Web.Script.Serialization;
using Standpoint.Core.Classes;
using Standpoint.Core.Utilities;
using Thinkgate.Base.Classes;
using Thinkgate.Base.Utilities;
using Thinkgate.Base.DataAccess;
using Thinkgate.Classes;
using Thinkgate.Classes.Serializers;
using Thinkgate.Controls.Addendums;
using Thinkgate.Controls.Assessment.ContentEditor;
using Thinkgate.Controls.E3Criteria;
using Thinkgate.Controls.Items;
using Thinkgate.Controls.Rubrics;
using Thinkgate.Utilities;
using System.Reflection;
using Thinkgate.Base.Enums;
using System.Linq;

using Thinkgate.ExceptionHandling;

namespace Thinkgate.Services
{
	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class Service2
	{
	    private int assessmentID;
	    private SessionObject sessionObject;

	    private int GetDecryptedId(string encryptedValue)
	    {
	        sessionObject = (SessionObject) HttpContext.Current.Session["SessionObject"];
	        var decryptedValue = Cryptography.DecryptionToInt(encryptedValue, sessionObject.LoggedInUser.CipherKey);
	        decryptedValue = decryptedValue != 0 ? decryptedValue : Encryption.DecryptStringToInt(encryptedValue);
	        return decryptedValue;
	    }
		//private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		// To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
		// To create an operation that returns XML,
		//     and include the following line in the operation body:
		//         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string UpdateItemOrders(string encryptedAssessmentID, int formId, string strSortPackage)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			int assessmentID = GetDecryptedId(encryptedAssessmentID);

			Assessment selectedAssessment = Assessment.UpdateAssessmentItemSort(assessmentID, formId, strSortPackage);
			// Add your operation implementation here)
			var return_JSON = new ActionResult();
			if (selectedAssessment != null)
			{
				//return_JSON.Status = "Updated Orders Attached";
				return_JSON.StatusCode = (int)ActionResult.GenericStatus.UpdatedDataAttached;
				return_JSON.PayLoad = selectedAssessment.PrepareFormsForJson();
			}

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return return_JSON.ToJSON();
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string GetItemJSON(string encryptedAssessmentID, int itemID)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            int assessmentID = GetDecryptedId(encryptedAssessmentID);

			var return_JSON = new ActionResult();

			var tq = TestQuestion.GetTestQuestionByID(itemID);

			//Start TFS Bug#260: 23-Nov-2012 : Sanjeev : Checking if TQ is null then return value should be null.

			if (tq == null)
			{
				return_JSON.PayLoad = null;
                Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
				return return_JSON.ToJSON();
			}

			//End TFS Bug#260: 23-Nov-2012 : Sanjeev : Checking if TQ is null then return value should be null.

			if (tq.TestID == assessmentID)
			{
				return_JSON.PayLoad = tq;
			}

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return return_JSON.ToJSON();
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string ManualReplace(string encryptedAssessmentID, int itemID, int newBankID)
		{
			int assessmentID = GetDecryptedId(encryptedAssessmentID);
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			
			var return_JSON = new ActionResult();

			var tq = Assessment.ManualReplace(assessmentID, itemID, newBankID, sessionObject.LoggedInUser.Page);
			if (tq == null)
			{
				return_JSON.StatusCode = (int)ActionResult.GenericStatus.GenericError;
				return_JSON.Message = "Error replacing item.";
			}
			else
			{
				if (tq.Responses.Count == 5) tq.Responses.RemoveAt(4); // we need field to indicate how many distractors an item has. Until then, we need to enforce 4 distractors for assessment content screen

				//tq.StandardName = StandardMasterList.GetStandardNameByID(tq.StandardID);
				tq.StandardName = Standards.GetStandardNameByID(tq.StandardID);
				tq.LoadRubric();
				tq.LoadAddendum();
				return_JSON.PayLoad = tq;
			}

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return return_JSON.ToJSON(new JavaScriptConverter[] { new ToStringSerializer(), new AddendumMinSerializer(), new RubricMinSerializer(), });
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string AutoReplace(string encryptedAssessmentID, int itemID)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			
			int assessmentID = GetDecryptedId(encryptedAssessmentID);
			Assessment selectedAssessment = Assessment.GetAssessmentByID(assessmentID);

			var return_JSON = new ActionResult();
			dtItemBank itemBanks = ItemBankMasterList.GetItemBanksForStandardSearch(sessionObject.LoggedInUser, selectedAssessment.TestCategory);

			var tq = selectedAssessment.AutoReplace(itemID, itemBanks, sessionObject.LoggedInUser.Page);
			if (tq == null)
			{
				return_JSON.StatusCode = (int)ActionResult.GenericStatus.GenericError;
				return_JSON.Message = "No replacement for this item could be found.";
			}
			else
			{
				if (tq.Responses.Count == 5) tq.Responses.RemoveAt(4); // we need field to indicate how many distractors an item has. Until then, we need to enforce 4 distractors for assessment content screen

				//tq.StandardName = StandardMasterList.GetStandardNameByID(tq.StandardID);
				tq.StandardName = Standards.GetStandardNameByID(tq.StandardID);
				tq.LoadRubric();
				tq.LoadAddendum();
				return_JSON.PayLoad = tq;
			}

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return return_JSON.ToJSON(new JavaScriptConverter[] { new ToStringSerializer(), new AddendumMinSerializer(), new RubricMinSerializer(), });
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string RemoveAssessmentItems(string encryptedAssessmentID, string itemsToRemove)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			
			int assessmentID = GetDecryptedId(encryptedAssessmentID);
			object returnPayload;
			try
			{
				var _serializer = new JavaScriptSerializer();
				//var assessmentForms = _serializer.Deserialize<IList<Thinkgate.Base.Classes.AssessmentForm>>(assessmentFormsStr);

				Assessment selectedAssessment = Assessment.GetAssessmentAndQuestionsByID(assessmentID);
				selectedAssessment.LoadForms(true);

				drGeneric_Int lstItemsToRemove = new drGeneric_Int(itemsToRemove, ',');
				drGeneric_Int lstRubricsToRemove = new drGeneric_Int();
				drGeneric_Int lstAddendumsToRemove = new drGeneric_Int();

				foreach (int questionID in lstItemsToRemove)
				{
					foreach (TestQuestion tq in selectedAssessment.Items.FindAll(x => x.ID == questionID))
					{
						if (tq.RubricID != 0 && tq.RubricID != null)
						{
							List<TestQuestion> questionsWithRubric = selectedAssessment.Items.FindAll(x => x.RubricID == tq.RubricID);
							List<int> lstRemoveItemsWithRubric = new List<int>();

							foreach (TestQuestion tqRubric in questionsWithRubric)
							{
								foreach (int i in lstItemsToRemove)
								{
									if (tqRubric.ID == i)
									{
										lstRemoveItemsWithRubric.Add(i);
									}
								}
							}

							if (questionsWithRubric.Count == lstRemoveItemsWithRubric.Count)
							{
								lstRubricsToRemove.Add(tq.RubricID);
							}
						}

						if (tq.AddendumID != 0 && tq.AddendumID != null)
						{
							List<TestQuestion> questionsWithAddendum = selectedAssessment.Items.FindAll(x => x.AddendumID == tq.AddendumID);
							List<int> lstRemoveItemsWithAddendum = new List<int>();

							foreach (TestQuestion tqAddendum in questionsWithAddendum)
							{
								foreach (int i in lstItemsToRemove)
								{
									if (tqAddendum.ID == i)
									{
										lstRemoveItemsWithAddendum.Add(i);
									}
								}
							}

							if (questionsWithAddendum.Count == lstRemoveItemsWithAddendum.Count)
							{
								lstAddendumsToRemove.Add(tq.AddendumID);
							}
						}
					}
				}

				selectedAssessment.RemoveItemsFromAssessment(lstItemsToRemove, lstRubricsToRemove, lstAddendumsToRemove);
				//DataTable dtNewWeights = Thinkgate.Base.Classes.Assessment.RecalculateWeights(assessmentID);
				returnPayload = new object[] { selectedAssessment.RenderNewWeightsForAssessmentDisplay(), _serializer.Serialize(selectedAssessment.PrepareFormsForJson()) };
			}
			catch (System.Exception e)
			{
				return new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message).ToJSON();
			}
			var return_JSON = new ActionResult();

			if (returnPayload != null)
			{
				return_JSON.PayLoad = returnPayload;
				return_JSON.ExecOnReturn = "update_weight_pcts(result.PayLoad[0])";
			}

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return return_JSON.ToJSON();
		}


		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string EraseItem(string encryptedAssessmentID, int itemID)
		{
			int assessmentID = GetDecryptedId(encryptedAssessmentID);
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			
			var return_JSON = new ActionResult();

			var tq = TestQuestion.EraseAssessmentItem(assessmentID, itemID);
			if (tq.TestID == assessmentID)
			{
				return_JSON.PayLoad = tq;
			}

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return return_JSON.ToJSON();
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string DeleteAssessment(string encryptedAssessmentID)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            int assessmentID = GetDecryptedId(encryptedAssessmentID);
            var return_JSON = new ActionResult();
			
			try
			{
                return_JSON = Thinkgate.Base.Classes.Assessment.DeleteAssessment(assessmentID);
			}
			catch (ApplicationException)
			{
				return new ActionResult((int)ActionResult.GenericStatus.GenericError).ToJSON();
			}

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return return_JSON.ToJSON();
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string ProofAssessmentMissingUploadedDoc(string encryptedAssessmentID)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            int assessmentID = GetDecryptedId(encryptedAssessmentID);
            var return_JSON = new ActionResult();
            			
			Assessment selectedAssessment = Assessment.GetAssessmentAndQuestionsByID(assessmentID);
            selectedAssessment.hasSecurePermission = sessionObject.LoggedInUser.HasPermission(Permission.Access_SecureTesting);
		    Dictionary<string, bool> dictionaryItem = TestTypes.TypeWithSecureFlag(selectedAssessment.Category.ToString());
            selectedAssessment.isSecuredFlag = dictionaryItem != null && dictionaryItem.Where(x => Boolean.Parse(x.Value.ToString())).Select(y => y.Key).ToList().Distinct().Any();
            
			try
			{                
                return_JSON = selectedAssessment.ProofAssessmentMissingUploadedDoc();
			}
			catch (ApplicationException)
			{
				return new ActionResult((int)ActionResult.GenericStatus.GenericError).ToJSON();
			}

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return return_JSON.ToJSON();
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string ProofAssessment(string encryptedAssessmentID, bool bypassLevel2Checks = false)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            int assessmentID = GetDecryptedId(encryptedAssessmentID);
            var return_JSON = new ActionResult();
            			
			Thinkgate.Base.Classes.Assessment selectedAssessment =
				Thinkgate.Base.Classes.Assessment.GetAssessmentAndQuestionsByID(assessmentID);
			try
			{
                return_JSON = selectedAssessment.ProofAssessment(bypassLevel2Checks);
			}
			catch (ApplicationException)
			{
				return new ActionResult((int)ActionResult.GenericStatus.GenericError).ToJSON();
			}

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return return_JSON.ToJSON();
		}

		/// <summary>
		/// This method attempts to mimic the AssessmentItemUpdate method however it allows a collection of changes to be passed in and updated.  
		/// It is limited in function in the following ways however: It only returns the last payload that was generated at this point.  Also 
		/// it is not set up to deal with result.ExecOnReturn scripts like in the case of when Item Weight is updated.  Please use the 
		/// AssessmentItemUpdate method if you are expecting something in your payload other than 0/Success or !=0/failure.
		/// </summary>
		/// <param name="AssessmentItemKeyValueCollectionJSON"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string AssessmentItemUpdateKeyValueFields(string AssessmentItemKeyValueCollectionJSON)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			var serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer() });
			var assessmentItemKeyValueCollection = Json.Decode(AssessmentItemKeyValueCollectionJSON, typeof(ItemKeyValueCollection));

			int assessmentID = GetDecryptedId(assessmentItemKeyValueCollection.EncAssessmentID);
			int itemID = DataIntegrity.ConvertToInt(assessmentItemKeyValueCollection.ItemID);
			var return_JSON = new ActionResult();
			object returnPayload;

			foreach (ItemKeyValueCollection.ItemInfoKeyVal iikv in assessmentItemKeyValueCollection.Attributes)
			{
				try
				{
					returnPayload = Thinkgate.Base.Classes.TestQuestion.UpdateField(assessmentID, itemID, 0, iikv.Key, iikv.Val);
				}
				catch (System.Exception e)
				{
					return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
				}
				if (returnPayload != null) return_JSON.PayLoad = (object)returnPayload;
			}

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return return_JSON.ToJSON();
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string AssessmentItemUpdateField(string encryptedAssessmentID, string itemID, string responseID, string field, string value)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			int assessmentID = GetDecryptedId(encryptedAssessmentID);
			if (field == "RubricID")
				value = Standpoint.Core.Classes.Encryption.DecryptString(value);
            value = AssessmentUtil.ReplaceSpecialCharToUnicode(value);
			object returnPayload;

			try
			{
				returnPayload = Thinkgate.Base.Classes.TestQuestion.UpdateField(assessmentID,
																				Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(itemID),
																				Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(responseID),
																				field, value);
			}
			catch (System.Exception e)
			{
				return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
			}
			var return_JSON = new ActionResult();
			if (returnPayload != null) return_JSON.PayLoad = (object)returnPayload;
			switch (field)
			{
				case "ScoreOnTest":
				case "ItemWeight":
					return_JSON.ExecOnReturn = "update_weight_pcts(result.PayLoad)";
					break;
			}

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return return_JSON.ToJSON();
		}


		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string AssessmentUpdateField(string encryptedAssessmentID, string field, string value)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			
			int assessmentID = GetDecryptedId(encryptedAssessmentID);
			object returnPayload;
            value = AssessmentUtil.ReplaceSpecialCharToUnicode(value);
			try
			{
				returnPayload = Thinkgate.Base.Classes.Assessment.UpdateField(assessmentID, field, value);
			}
			catch (System.Exception e)
			{
				return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
			}

			var return_JSON = new ActionResult();
			if (returnPayload != null) return_JSON.PayLoad = returnPayload;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return return_JSON.ToJSON();
		}


		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string DeleteItemImage(string encryptedImageID)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			
			int ImageID = GetDecryptedId(encryptedImageID);

			Thinkgate.Base.Classes.ItemImage.DeleteItemImage(ImageID);
			// Add your operation implementation here)

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return "Update Sent";
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string ItemImageUpdateField(string encryptedItemImageID, string field, string value)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			
			int ItemImageID = GetDecryptedId(encryptedItemImageID);
			object returnPayload;

			try
			{
				returnPayload = Thinkgate.Base.Classes.ItemImage.UpdateField(sessionObject.LoggedInUser.UserFullName,sessionObject.LoggedInUser.Page,ItemImageID, field, value);
			}
			catch (System.Exception e)
			{
				return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
			}

			var return_JSON = new ActionResult();
			if (returnPayload != null) return_JSON.PayLoad = returnPayload;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return return_JSON.ToJSON();
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string UpdateItem_ItemBank(string encryptedItemID, string contentType, string updateStr)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			
			int itemID = GetDecryptedId(encryptedItemID);
			object returnPayload;

			try
			{
				dtItemBank tempTable = new dtItemBank();
				string[] updateStrSplit = updateStr.Split('|');
				foreach (string ib in updateStrSplit)
				{
					if (ib.Length > 1)
					{
						string[] ibSplit = ib.Split('@');
						if (ibSplit.Length > 0)
						{
							tempTable.Add(DataIntegrity.ConvertToInt(ibSplit[0].ToString()), ibSplit[1].ToString(), 0, "");
						}
					}
				}
				int iContentType = DataIntegrity.ConvertToInt(contentType);
				returnPayload = Thinkgate.Base.Classes.ItemBank.ItemBanks_Update(iContentType, itemID, tempTable);
			}
			catch (System.Exception e)
			{
				return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
			}

			var returnJson = new ActionResult();
			if (returnPayload != null) returnJson.PayLoad = returnPayload;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return returnJson.ToJSON();
		}


		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string DeleteAddendum(string encryptedAddendumID)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			
			int AddendumID = GetDecryptedId(encryptedAddendumID);

			Thinkgate.Base.Classes.Addendum.DeleteAddendum(AddendumID);
			// Add your operation implementation here)

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return "Update Sent";
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string AddendumUpdateField(string encryptedAddendumID, string field, string value)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			
            int AddendumID = GetDecryptedId(encryptedAddendumID);
			object returnPayload;
            value = AssessmentUtil.ReplaceSpecialCharToUnicode(value);
			try
			{
                returnPayload = Thinkgate.Base.Classes.Addendum.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page,AddendumID, field, value);
			}
			catch (System.Exception e)
			{
				return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
			}

			var return_JSON = new ActionResult();
			if (returnPayload != null) return_JSON.PayLoad = returnPayload;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return return_JSON.ToJSON();
		}

		/// <summary>
		/// Creates the object according to ID and Type and then invokes that object's DeleteFromDatabase method.  
		/// </summary>
		/// <param name="encryptedData"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string DeleteBankQuestionFromDatabase(string encryptedData)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
		    var sDecryptedData = Cryptography.DecryptionToIntForService(encryptedData);

			int id = DataIntegrity.ConvertToInt(sDecryptedData);

			var oQuestion = BankQuestion.GetQuestionByID(id);
			oQuestion.DeleteFromDatabase();

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return "Delete successful.";
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string CopyItemToUserPersonalBank(string encryptedID)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            string results = string.Empty;

            results = Standpoint.Core.Classes.Encryption.EncryptInt(BankQuestion.CopyItemToUserPersonalBank(GetDecryptedId(encryptedID)));

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return results;
		}

		/// <summary>
		/// Creates the object according to ID and Type and then invokes that object's DeleteFromDatabase method.   
		/// </summary>
		/// <param name="encryptedData"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string DeleteRubricFromDatabase(string encryptedData)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			var sDecryptedData = Standpoint.Core.Classes.Encryption.DecryptString(encryptedData);

			int id = DataIntegrity.ConvertToInt(sDecryptedData);

			var oRubric = Rubric.GetRubricByID(id);

			string results = oRubric.DeleteFromDatabase();

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			if (results.Length == 0)
			{
				return "";
			}
			else
			{
				return results;
			}
		}

		/// <summary>
		/// Creates the object according to ID and Type and then invokes that object's DeleteFromDatabase method.   
		/// </summary>
		/// <param name="encryptedData"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string DeleteAddendumFromDatabase(string encryptedData)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			var sDecryptedData = Standpoint.Core.Classes.Encryption.DecryptString(encryptedData);

			int id = DataIntegrity.ConvertToInt(sDecryptedData);

			var oAddendum = Addendum.GetAddendumByID(id);

			string results = oAddendum.DeleteFromDatabase();

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			if (results.Length == 0)
			{
				return "";
			}
			else
			{
				return results;
			}
		}

		/// <summary>
		/// Creates the object according to ID and then invokes that object's DeleteFromDatabase method.  
		/// </summary>
		/// <param name="encryptedData"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string DeleteImageFromDatabase(string encryptedData)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			var sDecryptedData = Standpoint.Core.Classes.Encryption.DecryptString(encryptedData);

			int id = DataIntegrity.ConvertToInt(sDecryptedData);

			var oImage = ItemImage.GetImageByID(id);
			oImage.DeleteFromDatabase();

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return "Delete successful.";
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string BankItemUpdateKeyValueFields(string BankItemKeyValueCollectionJSON)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			var serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer() });
			var BankItemKeyValueCollection = Json.Decode(BankItemKeyValueCollectionJSON, typeof(ItemKeyValueCollection));

			int itemID = DataIntegrity.ConvertToInt(BankItemKeyValueCollection.ItemID);
			List<object> Payloads = new List<object>();
			var return_JSON = new ActionResult();
			object returnPayload;

            Classes.SessionObject sessionObject = (Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

			foreach (ItemKeyValueCollection.ItemInfoKeyVal iikv in BankItemKeyValueCollection.Attributes)
			{
				try
				{
					returnPayload = Thinkgate.Base.Classes.BankQuestion.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, itemID, 0, iikv.Key, iikv.Val);
				}
				catch (System.Exception e)
				{
					return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
				}
				if (returnPayload != null) Payloads.Add((object)returnPayload);
			}
			return_JSON.PayLoad = Payloads;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return return_JSON.ToJSON();
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string BankItemUpdateField(string encryptedItemID, string responseID, string field, string value)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			
			int itemID = GetDecryptedId(encryptedItemID);
			if (field == "RubricID")
				value = Standpoint.Core.Classes.Encryption.DecryptString(value);
            value = AssessmentUtil.ReplaceSpecialCharToUnicode(value);
			object returnPayload;

			try
			{
				returnPayload = Thinkgate.Base.Classes.BankQuestion.UpdateField(
                    sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page,
					Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(itemID),
					Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(responseID),
					field, value);
			}
			catch (System.Exception e)
			{
				return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
			}
			var return_JSON = new ActionResult();
			if (returnPayload != null) return_JSON.PayLoad = (object)returnPayload;
			switch (field)
			{
				case "ScoreOnTest":
				case "ItemWeight":
					//return_JSON.ExecOnReturn = "update_weight_pcts(result.PayLoad)";
					break;
			}

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return return_JSON.ToJSON();
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string ItemInsertStandard(string encryptedAssessmentID, string encryptedItemID, string standardList)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            int assessmentID = GetDecryptedId(encryptedAssessmentID);
            int itemID = GetDecryptedId(encryptedItemID);
			
			string standardID = "";
			object returnPayload;

			var dtObj = new dtGeneric_String_Int();
			string[] values = standardList.Split('|');
			int counter = 1;
			foreach (var value in values)
			{
				string[] subvalues = value.Split(',');
				dtObj.Add(subvalues[0], GetDecryptedId(subvalues[1]));
				if (counter == 1)
				{
					standardID = Standpoint.Core.Classes.Encryption.DecryptString(subvalues[1]);
				}
				counter++;
			}

			try
			{
				if (assessmentID > 0)
				{
					returnPayload = TestQuestion.UpdateField(assessmentID,
																					DataIntegrity.ConvertToInt(itemID),
																					DataIntegrity.ConvertToInt(null),
																					"StandardID", standardID);
				}
				else
				{
					returnPayload = BankQuestion.AddStandardsToBankItem(DataIntegrity.ConvertToInt(itemID), dtObj);
				}
			}
			catch (Exception e)
			{
				return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
			}

			var returnJSON = new ActionResult();
			if (returnPayload != null) returnJSON.PayLoad = returnPayload;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return returnJSON.ToJSON();
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string ItemRemoveStandard(string encryptedAssessmentID, string encryptedItemID, string encryptedStandardID)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            int assessmentID = GetDecryptedId(encryptedAssessmentID);
            int itemID = GetDecryptedId(encryptedItemID);
            int standardID = GetDecryptedId(encryptedStandardID);
			
			object returnPayload;
			try
			{
				returnPayload = assessmentID > 0 ? null : BankQuestion.RemoveStandardFromBankItem(DataIntegrity.ConvertToInt(itemID), DataIntegrity.ConvertToInt(standardID));
			}
			catch (Exception e)
			{
				return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
			}

			var returnJSON = new ActionResult();
			if (returnPayload != null) returnJSON.PayLoad = returnPayload;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return returnJSON.ToJSON();
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string ItemImageSearchV2(bool prefetch, string pageNumber, int returnCount, string criteriaControllerJSON, string selectedSortField)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			var serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer() });
			var criteriaController = Json.Decode(criteriaControllerJSON, typeof(CriteriaController));

			var sessionObject = (Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

			ItemImageSearchCriteriaV2 criteriaObject = Controls.Images.ImageSearch_ExpandedV2.BuildSearchCriteriaObject(sessionObject.LoggedInUser, criteriaController, selectedSortField);
			var images = ItemImage.GetImagesByCriteria(criteriaObject, DataIntegrity.ConvertToInt(pageNumber), returnCount);

			var imagesList = images.ItemImageList;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return serializer.Serialize(imagesList);
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string QuestionSearch(bool prefetch, string pageNumber, string returnCount, string criteriaControllerJSON, string selectedSortField, string itemSearchMode, string testYear, int testCurrCourseID)
		{
            try
            {
                Log("Service2.svc --> QuestionSearch", true);
                
			var serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer(), new StandardsSerializer() });
			var criteriaController = Json.Decode(criteriaControllerJSON, typeof(CriteriaController));

			Classes.SessionObject sessionObject = (Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

			var criteriaObject = ItemSearch.BuildSearchCriteriaObject(sessionObject.LoggedInUser, criteriaController, selectedSortField);
			var questions = Questions.Search(criteriaObject, DataIntegrity.ConvertToInt(pageNumber), 100, itemSearchMode == "MultiSelect" || itemSearchMode == "SingleSelect", testYear, testCurrCourseID);


			IList<BankQuestion> questionsList = questions.BankQuestionsList;
                Log("Service2.svc --> QuestionSearch", false);
                return serializer.Serialize(questionsList);

            }
            catch (Exception ex)
            {
                Log("Service2.svc --> QuestionSearch -> " + ex.StackTrace, false);
                throw new Exception(ex.Message);
                return null;
            }
           
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string AddendumSearchV2(bool prefetch, string pageNumber, string returnCount, string criteriaControllerJSON, string selectedSortField)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			var serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer() });
			var criteriaController = Json.Decode(criteriaControllerJSON, typeof(CriteriaController));

			var sessionObject = (Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

			var criteriaObject = AddendumSearch_ExpandedV2.BuildSearchCriteriaObject(sessionObject.LoggedInUser, criteriaController, selectedSortField);
			var addendums = Addendum.GetAddendumsByCriteriaPaging(criteriaObject, DataIntegrity.ConvertToInt(pageNumber), 100);

			var addendumsList = addendums.AddendumList;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return serializer.Serialize(addendumsList);
		}


		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string RubricSearchV2(bool prefetch, string pageNumber, string returnCount, string criteriaControllerJSON, string selectedSortField)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			var serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer() });
			var criteriaController = Json.Decode(criteriaControllerJSON, typeof(CriteriaController));

			var sessionObject = (Classes.SessionObject)System.Web.HttpContext.Current.Session["SessionObject"];

			var criteriaObject = RubricSearch_ExpandedV2.BuildSearchCriteriaObject(sessionObject.LoggedInUser, criteriaController, selectedSortField);
			var rubrics = Rubric.GetRubricsByCriteriaPaging(criteriaObject, DataIntegrity.ConvertToInt(pageNumber), RubricSearch_ExpandedV2.RecordsPerPage);

			var rubricsList = rubrics.RubricList;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return serializer.Serialize(rubricsList);
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string RubricSearch(bool prefetch, string pageNumber, string returnCount, Base.Classes.DataContracts.RubricSearchCriteria rubricSearchCriteria)
		{
			var itemBanks = new dtItemBank();
			if (rubricSearchCriteria.ItemBanks != null)
			{
				foreach (Base.Classes.DataContracts.ItemBankDataContract ibdc in rubricSearchCriteria.ItemBanks.ItemBanks)
				{
					itemBanks.Rows.Add(
						ibdc.TargetType,
						ibdc.Target,
						ibdc.ApprovalSource,
						ibdc.Label);
				}
			}


			RubricSearchCriteria qsc = new RubricSearchCriteria(rubricSearchCriteria.StandardCourses, itemBanks, rubricSearchCriteria.RubricTypes, rubricSearchCriteria.CreatedDateRange, rubricSearchCriteria.TextWords, rubricSearchCriteria.TextWordsOpt, rubricSearchCriteria.Page, rubricSearchCriteria.RecordCount, rubricSearchCriteria.SortKeyword);

			var rubric = Rubric.GetRubricsByCriteriaPaging(qsc, DataIntegrity.ConvertToInt(pageNumber), DataIntegrity.ConvertToInt(returnCount));

			IList<Rubric> rubricsList = rubric.RubricList;

			var serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new JavaScriptConverter[] { new ToStringSerializer() });

			string renderItemsScript = serializer.Serialize(rubricsList);

			return renderItemsScript;
		}


		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string GetBankItemJSONById(int itemID)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			BankQuestion question = BankQuestion.GetQuestionByID(itemID);

			int position = question.Responses != null ? question.Responses.Count - 1 : 0;

			while (position > 0)
			{
				if (string.IsNullOrEmpty(question.Responses[position].DistractorText))
				{
					question.Responses.Remove(question.Responses[position]);
					break;
				}
				position--;
			}

			var serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new JavaScriptConverter[] { new StandardsSerializer() });
			string renderItemsScript = serializer.Serialize(question);

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return renderItemsScript;
		}

		
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string RubricUpdateField(string encryptedRubricID, string field, string value)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            int rubricID = GetDecryptedId(encryptedRubricID);
			
            value = AssessmentUtil.ReplaceSpecialCharToUnicode(value);
			object returnPayload;

			try
			{
				returnPayload = Rubric.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, rubricID, field, value);
			}
			catch (Exception e)
			{
				return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
			}

			var returnJson = new ActionResult();
			if (returnPayload != null) returnJson.PayLoad = returnPayload;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return returnJson.ToJSON();
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string RubricItemUpdateField(string encryptedRubricID, string sRubricItemID, string field, string value)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            int rubricID = GetDecryptedId(encryptedRubricID);
			
			int rubricItemID = DataIntegrity.ConvertToInt(sRubricItemID);
            value = AssessmentUtil.ReplaceSpecialCharToUnicode(value);
			object returnPayload;

			try
			{
				returnPayload = RubricItems.UpdateField(rubricID, rubricItemID, field, value);
			}
			catch (Exception e)
			{
				return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
			}

			var returnJson = new ActionResult();
			if (returnPayload != null) returnJson.PayLoad = returnPayload;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return returnJson.ToJSON();
		}

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string RubricUpdatePointsAndCriteria(string encryptedRubricID, string points, string criteria)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            int rubricID = GetDecryptedId(encryptedRubricID);
			
			object returnPayload;

			try
			{
                returnPayload = Rubric.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, rubricID, "Points", points);
                returnPayload = Rubric.UpdateField(sessionObject.LoggedInUser.UserFullName, sessionObject.LoggedInUser.Page, rubricID, "Criteria", criteria);
			}
			catch (Exception e)
			{
				return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
			}

			var returnJson = new ActionResult();
			if (returnPayload != null) returnJson.PayLoad = returnPayload;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return returnJson.ToJSON();
		}


		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string RubricGetRubricItems(string encryptedRubricID)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            int rubricID = GetDecryptedId(encryptedRubricID);
			
			Rubric returnPayload;

			try
			{
				returnPayload = Rubric.GetRubricByID(rubricID);
			}
			catch (Exception e)
			{
				return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
			}

			var returnJson = new ActionResult();
			if (returnPayload != null) returnJson.PayLoad = returnPayload.RubricItemsList;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return returnJson.ToJSON();
		}

		/// <summary>
		/// When passed a legitimate Rubric ID, this method instantiates the rubric object and passes back
		/// to the caller the html-formatted content of the rubric.
		/// </summary>
		/// <param name="encryptedRubricID"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string RubricGetDirectionsAndContentFormatted(string encryptedRubricID)
		{
			if (string.IsNullOrEmpty(encryptedRubricID)) return null;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            int rubricID = GetDecryptedId(encryptedRubricID);
			

		    var serializer = new JavaScriptSerializer();
			serializer.RegisterConverters(new JavaScriptConverter[] { new StandardsSerializer() });

			var oRubric = Rubric.GetRubricByID(rubricID);
			//var renderRubricDirectionScript = new StringBuilder("<div style=\"border: 1px solid black; margin-bottom: 10px;\"><div style=\"text-align: center; margin-bottom: 10px\">Rubric Directions</div><div>" +
			//                           oRubric.Directions +
			//                           "</div></div>");
			//var renderRubricDirectionScript = serializer.Serialize(oRubric.Directions);
			//var renderRubricContentScript = serializer.Serialize(oRubric.Content);

			var renderRubricDirectionScript = oRubric.Directions;
			var renderRubricContentScript = oRubric.Content;

			var returnJson = new ActionResult();
			returnJson.PayLoad = new object[] { renderRubricDirectionScript, renderRubricContentScript };

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return returnJson.ToJSON();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="encryptedRubricID"></param>
		/// <returns></returns>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public static void KillSession()
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

			expireCookie(".KENTICOAUTH");
			System.Web.Security.FormsAuthentication.SignOut();
			System.Web.HttpContext.Current.Session["SessionObject"] = null;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);

			System.Web.HttpContext.Current.Response.Redirect("~/TGLogin.aspx", true);
		}

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public static void KillSessionSSO()
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

            expireCookie(".KENTICOAUTH");
            System.Web.Security.FormsAuthentication.SignOut();
            HttpContext.Current.Session["SessionObject"] = null;
            HttpContext.Current.Session.Abandon();

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
        }

		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public void Logout()
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

			expireCookie(".KENTICOAUTH");
			System.Web.HttpContext.Current.Session["SessionObject"] = null;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
		}

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public void UpdateStudentDemographics(int studentID, string demoFields)
        {
            dtGeneric_String_String gssDemoFields = new dtGeneric_String_String();
			string[] fields = demoFields.Split('|');
			foreach (var field in fields)
			{
				string[] values = field.Split(',');
				gssDemoFields.Add(values[0], values[1]);
			}

            Base.Classes.Data.StudentDB.UpdateStudentDemographics(studentID, gssDemoFields);
        }

		public static void expireCookie(string cookieToExpire)
		{
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);

			if (System.Web.HttpContext.Current.Request.Cookies[cookieToExpire] != null)
			{
				System.Web.HttpCookie myCookie = System.Web.HttpContext.Current.Request.Cookies[cookieToExpire];
				myCookie.Expires = DateTime.Now.AddDays(-1d);
				System.Web.HttpContext.Current.Response.Cookies.Add(myCookie);
			}

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
		}

		/// <summary>
		/// Gets Addendum full text based on AddendumId and AssessmentId.
		/// </summary>
		/// <param name="xId">Addendum Id</param>
		/// <param name="assessmentId">Assessment Id</param>
		/// <returns>The Addendum full text.</returns>
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
		[OperationContract]
		public string GetAddedendumText(string xId, string assessmentId)
		{
			string sItemId = string.Empty + xId;

			if (string.IsNullOrWhiteSpace(sItemId))
			{ return string.Empty; }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
			string sAssessmentId = string.Empty + assessmentId;
			string addendumContent = string.Empty;
			Addendum addendum = null;

			int iItemId = GetDecryptedId(sItemId);
			if (string.IsNullOrWhiteSpace(sAssessmentId))
			{
				addendum = Addendum.GetAddendumByBankItemID(iItemId);
			}
			else
			{
				int iAssessmentId = GetDecryptedId(sAssessmentId);
				addendum = Addendum.GetAddendumByAssessmentIDAndItemID(iItemId, iAssessmentId);
			}

			if (addendum != null)
			{ addendumContent = addendum.Addendum_Text; }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
			return (new JavaScriptSerializer()).Serialize(addendumContent);
		}

        /// <summary>
        /// Performs an action on update of Assessment Item. Either updated item is treated as correction or add this as new item.
        /// </summary>
        /// <param name="itemId">Item ID</param>
        /// <param name="itemUpdateAction">Item Update Action</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public string AssessmentItemUpdateAction(string itemId, string updateAction)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            object returnPayload;
            try
            {
                returnPayload = Thinkgate.Base.Classes.TestQuestion.ItemUpdateAction(Standpoint.Core.Utilities.DataIntegrity.ConvertToInt(itemId),
                                                                                updateAction);
            }
            catch (System.Exception e)
            {
                return (new ActionResult((int)ActionResult.GenericStatus.GenericError, e.Message)).ToJSON();
            }

            var returnJson = new ActionResult();
            if (returnPayload != null) returnJson.PayLoad = (object)returnPayload;

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return returnJson.ToJSON();
        }

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public string AssessmentItemDistractorOrderUpdateField(string encryptedAssessmentId, int itemId, int correctAnswer, string newOrder)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            int assessmentID = GetDecryptedId(encryptedAssessmentId);

            Assessment selectedAssessment = Assessment.UpdateAssessmentItemDistractorSort(assessmentID, itemId, correctAnswer, newOrder);

            var return_JSON = new ActionResult();
            if (selectedAssessment != null)
            {
                return_JSON.StatusCode = (int)ActionResult.GenericStatus.UpdatedDataAttached;
                return_JSON.PayLoad = selectedAssessment.PrepareFormsForJson();
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return return_JSON.ToJSON();
        }

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public string BankItemDistractorOrderUpdateField(int itemId, int correctAnswer, string newOrder)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            Assessment selectedAssessment = Assessment.UpdateBankItemDistractorSort(itemId, correctAnswer, newOrder);

            var returnJson = new ActionResult();
            if (selectedAssessment != null)
            {
                returnJson.StatusCode = (int)ActionResult.GenericStatus.UpdatedDataAttached;
                returnJson.PayLoad = selectedAssessment.PrepareFormsForJson();
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return returnJson.ToJSON();
        }        

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public string CheckDefaultSortOrder(int formId, int itemId)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            string addendum_AssessmentItemCount = Thinkgate.Base.Classes.TestQuestion.CheckDefaultSort(formId, itemId);
        
            var returnJson = new ActionResult();
            if (addendum_AssessmentItemCount != null)
            {
                returnJson.StatusCode = (int)ActionResult.GenericStatus.UpdatedDataAttached;
                returnJson.PayLoad = addendum_AssessmentItemCount;
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return returnJson.ToJSON();
        }

        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public string UseDefaultSortOrder(int formId, int itemId)
        {
            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, true);
            bool updateSuccess = false;
            updateSuccess = Thinkgate.Base.Classes.TestQuestion.UseDefaultSort(formId, itemId);
            
            var returnJson = new ActionResult();
            if (updateSuccess == true)
            {
                returnJson.StatusCode = (int)ActionResult.GenericStatus.UpdatedDataAttached;
                returnJson.PayLoad = Convert.ToString(updateSuccess);
            }

            Log(MethodBase.GetCurrentMethod().DeclaringType.ToString() + "->" + MethodBase.GetCurrentMethod().Name, false);
            return returnJson.ToJSON();
        }
		// Add more operations here and mark them with [OperationContract]

        private static void Log(string logger, bool isStart)
        {
            if (isStart)
            { ThinkgateEventSource.Log.WCFServiceStart(logger, "request Service2WCF", "Service2WCF"); }
            else
            { ThinkgateEventSource.Log.WCFServiceEnd(logger, "end Service2WCF", "Service2WCF"); }
        }
	}
}
