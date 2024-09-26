// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class HasLanguageHelper
    {        
        private static void FixDetailBaseAndIntroText(Detail mydetail)
        {
            if (!String.IsNullOrEmpty(mydetail.BaseText))
            {
                mydetail.BaseText = mydetail.BaseText.Trim();

                if (mydetail.BaseText == "&#10;&#10;" || mydetail.BaseText == "\n\n")
                    mydetail.BaseText = "";
            }
            if (!String.IsNullOrEmpty(mydetail.IntroText))
            {
                mydetail.IntroText = mydetail.IntroText.Trim();

                if (mydetail.IntroText == "&#10;&#10;" || mydetail.IntroText == "\n\n")
                    mydetail.IntroText = "";
            }
        }

        private static void FixDetailLanguageField(ILanguage mydetail, string language)
        {
            if (String.IsNullOrEmpty(mydetail.Language))
                mydetail.Language = language;
        }

        //Generic
        public static void CheckMyInsertedLanguages<T>(this T mypoiactivity, List<string>? availablelanguages) where T : IHasLanguage, IContactInfosAware, IDetailInfosAware
        {
            if (availablelanguages == null || availablelanguages.Count == 0)
                availablelanguages = new List<string>() { "de", "it", "en", "nl", "cs", "pl", "fr", "ru" };

            if (mypoiactivity.HasLanguage == null)
                mypoiactivity.HasLanguage = new List<string>();

            //Detail, ImageGallery, ContactInfos, AdditionalArticleInfos, 
            foreach (string language in availablelanguages)
            {
                if (mypoiactivity.Detail.ContainsKey(language) || mypoiactivity.ContactInfos.ContainsKey(language)) // || mypoiactivity.AdditionalPoiInfos.ContainsKey(language))
                {
                    bool removelang = true;

                    if (mypoiactivity.Detail.ContainsKey(language) && mypoiactivity.Detail[language] != null)
                    {
                        var detailvalues = mypoiactivity.Detail[language];

                        FixDetailLanguageField(detailvalues, language);
                        FixDetailBaseAndIntroText(detailvalues);

                        if (!String.IsNullOrEmpty(detailvalues.AdditionalText))
                            removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.BaseText))
                            if (!String.IsNullOrEmpty(detailvalues.BaseText.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.GetThereText))
                            removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.Header))
                            removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.IntroText))
                            if (!String.IsNullOrEmpty(detailvalues.IntroText.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.SubHeader))
                            removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.Title))
                            removelang = false;
                    }

                    if (mypoiactivity.ContactInfos.ContainsKey(language) && mypoiactivity.ContactInfos[language] != null)
                    {
                        var contactvalues = mypoiactivity.ContactInfos[language];

                        FixDetailLanguageField(contactvalues, language);

                        if (!String.IsNullOrEmpty(contactvalues.Address))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.City))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CompanyName))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CountryCode))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CountryName))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Email))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Faxnumber))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Givenname))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.LogoUrl))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.NamePrefix))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Phonenumber))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Surname))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Tax))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Url))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Vat))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.ZipCode))
                            removelang = false;
                    }

                    //Add Language
                    if (removelang == false)
                    {
                        if (!mypoiactivity.HasLanguage.Contains(language))
                            mypoiactivity.HasLanguage.Add(language);
                    }
                    //Remove Language
                    else if (removelang == true)
                    {
                        if (mypoiactivity.Detail.ContainsKey(language))
                            mypoiactivity.Detail.Remove(language);

                        if (mypoiactivity.ContactInfos.ContainsKey(language))
                            mypoiactivity.ContactInfos.Remove(language);

                        if (mypoiactivity.HasLanguage.Contains(language))
                            mypoiactivity.HasLanguage.Remove(language);
                    }
                }
            }
        }


        //Check Language for Example DataModel
        public static void CheckMyInsertedLanguages(this Example example, List<string> availablelanguages)
        {
            if (example.HasLanguage == null)
                example.HasLanguage = new List<string>();

            //Detail, ImageGallery, ContactInfos, AdditionalArticleInfos, 
            foreach (string language in availablelanguages)
            {
                if (example.Detail.ContainsKey(language) || example.ContactInfos.ContainsKey(language))
                {
                    bool removelang = true;
                    
                    if (example.Detail.ContainsKey(language) && example.Detail[language] != null)
                    {
                        var detailvalues = example.Detail[language];

                        FixDetailLanguageField(detailvalues, language);
                        FixDetailBaseAndIntroText(detailvalues);

                        if (!String.IsNullOrEmpty(detailvalues.AdditionalText))
                            if (!String.IsNullOrEmpty(detailvalues.AdditionalText.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.BaseText))
                            if (!String.IsNullOrEmpty(detailvalues.BaseText.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.GetThereText))
                            if (!String.IsNullOrEmpty(detailvalues.GetThereText.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.Header))
                            if (!String.IsNullOrEmpty(detailvalues.Header.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.IntroText))
                            if (!String.IsNullOrEmpty(detailvalues.IntroText.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.SubHeader))
                            if (!String.IsNullOrEmpty(detailvalues.SubHeader.Trim()))
                                removelang = false;
                        if (!String.IsNullOrEmpty(detailvalues.Title))
                            if (!String.IsNullOrEmpty(detailvalues.Title.Trim()))
                                removelang = false;

                        example.Detail[language].Language = language;
                    }
                    
                    if (example.ContactInfos.ContainsKey(language) && example.ContactInfos[language] != null)
                    {
                        var contactvalues = example.ContactInfos[language];

                        FixDetailLanguageField(contactvalues, language);

                        if (!String.IsNullOrEmpty(contactvalues.Address))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.City))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CompanyName))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CountryCode))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.CountryName))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Email))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Faxnumber))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Givenname))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.LogoUrl))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.NamePrefix))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Phonenumber))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Surname))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Tax))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Url))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.Vat))
                            removelang = false;
                        if (!String.IsNullOrEmpty(contactvalues.ZipCode))
                            removelang = false;

                        example.ContactInfos[language].Language = language;
                    }
                    
                    //Add Language
                    if (removelang == false)
                    {
                        if (!example.HasLanguage.Contains(language))
                            example.HasLanguage.Add(language);
                    }
                    //Remove Language
                    else if (removelang == true)
                    {
                        if (example.Detail.ContainsKey(language))
                            example.Detail.Remove(language);

                        if (example.ContactInfos.ContainsKey(language))
                            example.ContactInfos.Remove(language);

                        if (example.HasLanguage.Contains(language))
                            example.HasLanguage.Remove(language);
                    }
                }
            }            
        }
    }
}
