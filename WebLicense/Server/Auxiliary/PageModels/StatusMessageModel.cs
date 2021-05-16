using System;
using Newtonsoft.Json;

namespace WebLicense.Server.Auxiliary.PageModels
{
    [Serializable]
    public class StatusMessageModel
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public StatusMessageModel() : this(null, true)
        {
        }

        public StatusMessageModel(string message, bool success)
        {
            Message = !string.IsNullOrWhiteSpace(message) ? message?.Trim() : null;
            IsSuccess = success;
        }

        #region Methods

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Message);
        }

        public bool NotIsEmpty()
        {
            return !IsEmpty();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static StatusMessageModel FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new StatusMessageModel();

            try
            {
                return JsonConvert.DeserializeObject<StatusMessageModel>(json);
            }
            catch (Exception)
            {
                return new StatusMessageModel();
            }
        }

        #endregion
    }
}