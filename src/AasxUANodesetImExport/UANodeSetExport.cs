/* 
Copyright (c) 2018-2020 Festo AG & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

Copyright (c) 2020 Phoenix Contact GmbH & Co. KG <opensource@phoenixcontact.com> 
Author: Andreas Orzelski

Copyright (c) 2020 Fraunhofer IOSB-INA Lemgo, 
    eine rechtlich nicht selbständige Einrichtung der Fraunhofer-Gesellschaft
    zur Förderung der angewandten Forschung e.V. <jan.nicolas.weskamp@iosb-ina.fraunhofer.de>
Author: Jan Nicolas Weskamp
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using AdminShellNS;
using static AdminShellNS.AdminShellV20;

// TODO (Michael Hoffmeister, 1970-01-01): License
// TODO (Michael Hoffmeister, 1970-01-01): Fraunhofer IOSB: Check ReSharper

// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantCast
// ReSharper disable RedundantCast
// ReSharper disable PossibleIntendedRethrow
// ReSharper disable SimplifyConditionalTernaryExpression
// ReSharper disable UnusedVariable


namespace AasxUANodesetImExport
{
    public static class UANodeSetExport
    {
        //consists of every single node that will be created
        public static List<UANode> root = new List<UANode>();

        private static int masterID = 7500;
        private static string typesNS = "http://admin-shell.io/OPC_UA_CS/Types.xsd";

        public static UANodeSet getInformationModel(string filename)
        {
            string path = filename;
            UANodeSet InformationModel = new UANodeSet();
            string executebaleLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string location = Path.Combine(executebaleLocation, path);
            string xml = System.IO.File.ReadAllText(path);

            XmlSerializer serializer = new XmlSerializer(typeof(UANodeSet));
            using (TextReader reader = new StringReader(xml))
            {
                try
                {
                    InformationModel = (UANodeSet)serializer.Deserialize(reader);
                }
                catch (Exception ex)
                {
                    MessageBoxResult result = MessageBox.Show("Error in XML formatting\n\n" + ex.ToString(),
                                          "Error",
                                          MessageBoxButton.OK);
                    throw ex;
                }

            }
            return InformationModel;
        }

        //Annotations
        //Almost every Method is build the same way:
        //1. Create UANode object and set its parameters
        //   Create a List of References and add a Reference to the Type (HasTypeDefinition)
        //   Increment masterId++, so the InformationModel will stay consistent and so there be no duplicate IDs
        //2. Set specific parameters of the mapped Object with properties
        //   createReference only needs the ReferenceType and a nodeId, that is why every method only returns a string
        //3. Create an array from the List
        //   Add the created Node to the root List

        public static string CreateAAS(string name, AdminShellV20.AdministrationShellEnv env)
        {

            UAObject sub = new UAObject();
            sub.NodeId = "ns=1;i=" + masterID.ToString();
            sub.BrowseName = "1:AASAssetAdministrationShell";
            masterID++;
            List<Reference> refs = new List<Reference>();

            refs.Add(new Reference
            {
                IsForward = false,
                Value = "i=85",
                ReferenceType = "Organizes"
            });
            refs.Add(CreateHasTypeDefinition("1:AASAssetAdministrationShellType"));

            //add ConceptDictionary
            refs.Add(CreateReference("HasComponent", CreateConceptDictionaryFolder(env.ConceptDescriptions)));

            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/AssetAdministrationShell"));

            //add Assets
            foreach (AdminShellV20.Asset asset in env.Assets)
            {
                refs.Add(CreateReference("HasComponent", CreateAASAsset(asset, "3:http://admin-shell.io/aas/2/0/AssetAdministrationShell/asset")));
            }

            //add Submodels
            foreach (AdminShellV20.Submodel submodel in env.Submodels)
            {
                string id = CreateAASSubmodel(submodel, "3:http://admin-shell.io/aas/2/0/AssetAdministrationShell/submodels");
                refs.Add(CreateReference("HasComponent", id));
            }

            //map AAS Information
            foreach (AdminShellV20.AdministrationShell shell in env.AdministrationShells)
            {
                if (shell.views != null)
                {
                    foreach (AdminShellV20.View view in shell.views.views)
                    {
                        refs.Add(CreateReference("HasComponent", CreateView(view, "3:http://admin-shell.io/aas/2/0/AssetAdministrationShell/views")));
                    }
                }

                if (shell.derivedFrom != null)
                {
                    refs.Add(CreateReference("HasComponent", CreateDerivedFrom(shell.derivedFrom.Keys)));
                }

                if (shell.idShort != null)
                {
                    refs.Add(
                        CreateReference(
                            "HasProperty",
                            CreateProperty(shell.idShort, "PropertyType", "idShort", "String", "")));
                }

                if (shell.administration != null && shell.identification != null) refs.Add(CreateReference("HasInterface", CreateIdentifiable(shell.identification.id, shell.identification.idType, shell.administration.version, shell.administration.revision)));

                if (shell.hasDataSpecification != null)
                {
                    refs.Add(CreateReference("HasComponent", CreateHasDataSpecification(shell.hasDataSpecification)));
                }

                if (shell.assetRef != null)
                {
                    refs.Add(CreateReference("HasComponent", CreateAssetRef(shell.assetRef, "")));
                }
            }

            sub.References = refs.ToArray();
            root.Add((UANode)sub);
            return sub.NodeId;
        }


        private static string CreateAASSubmodel(AdminShellV20.Submodel submodel, string dic)
        {
            UAObject sub = new UAObject();
            sub.NodeId = "ns=1;i=" + masterID.ToString();
            sub.BrowseName = "1:" + "Submodel:" + submodel.idShort;
            masterID++;
            List<Reference> refs = new List<Reference>();
            if (submodel.kind != null)
                //refs.Add( CreateReference("HasProperty",CreateProperty(GetAASModelingKindDataType(submodel.kind.kind).ToString(), "PropertyType", "ModellingKind", "AASModelingKindDataType")));

            refs.Add(CreateHasTypeDefinition("1:AASSubmodelType"));
            
            if(!string.IsNullOrWhiteSpace(submodel.category)) refs.Add(CreateReference( "HasProperty", CreateProperty(submodel.category, "PropertyType", "Category", "String", "")));

            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/Submodel"));
            refs.Add(CreateHasDictionaryEntry(dic));

            if (submodel.identification.idType == "IRI")
            {
                refs.Add(CreateReference("HasDictionaryEntry", CreateIriConceptDescription(submodel.semanticId, "")));
            }
            else
            {
                refs.Add(CreateReference("HasDictionaryEntry", CreateIrdiConceptDescription(submodel.semanticId, "")));
            }


            //set Identifiable
            if (submodel.administration == null)
            {
                refs.Add(
                    CreateReference(
                        "HasInterface",
                        CreateIdentifiable(submodel.identification.id, submodel.identification.idType, null, null)));
            }
            else if (submodel.identification == null)
            {
                refs.Add(
                    CreateReference(
                        "HasInterface",
                        CreateIdentifiable(null, null, submodel.administration.version,
                            submodel.administration.revision)));

            }
            else
            {
                refs.Add(
                    CreateReference(
                        "HasInterface",
                        CreateIdentifiable(submodel.identification.id, submodel.identification.idType, submodel.administration.version, submodel.administration.revision)));
            }

            //set Qualifier if it exists
            if (submodel.qualifiers != null)
            {
                foreach (AdminShellV20.Qualifier qualifier in submodel.qualifiers)
                {
                    refs.Add(
                        CreateReference(
                            "HasComponent", CreateAASQualifier(qualifier.type, qualifier.value, qualifier.valueType, qualifier.semanticId , "3:http://admin-shell.io/aas/2/0/Submodel/qualifiers")));
                }
            }

            //add Elements
            foreach (AdminShellV20.SubmodelElementWrapper element in submodel.submodelElements)
            {
                string id = CreateSubmodelElement(element.submodelElement, "3:http://admin-shell.io/aas/2/0/Submodel/submodelElements");
                refs.Add(CreateReference("HasComponent", id));
            }

            sub.References = refs.ToArray();
            root.Add((UANode)sub);
            return sub.NodeId;
        }

        private static string CreateProperty(string value, string type, string BrowseName, string datatype, string dic)
        {
            //Creates a Property with a single Value

            UAVariable prop = new UAVariable();
            prop.NodeId = "ns=1;i=" + masterID.ToString();
            prop.BrowseName = "1:" + BrowseName;
            prop.DataType = datatype;
            masterID++;
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            //Remove ':', because it's not allowed in XML
            if (datatype.Contains(':'))
            {
                var strings = datatype.Split(':');
                datatype = strings[1];
            }

            var isNumeric = int.TryParse(value, out int n);
            if (isNumeric)
            {
                //Create XMLElement to store Value in
                System.Xml.XmlElement element = doc.CreateElement("uax", "Int32", "");
                element.InnerText = value;
                prop.Value = element;
            }
            else
            {
                //Create XMLElement to store Value in
                System.Xml.XmlElement element = doc.CreateElement( "uax", datatype, "http://opcfoundation.org/UA/2008/02/Types.xsd");
                element.InnerText = value;
                prop.Value = element;
            }



            prop.References = new Reference[2];

            prop.References[0] = CreateHasTypeDefinition(type);
            prop.References[1] = CreateHasDictionaryEntry(dic);
            root.Add((UANode)prop);

            return prop.NodeId;
        }

        private static Reference CreateReference(string type, string value)
        {
            if (value != null)
            {
                //check if Type is "Base"Type (not seperately created)
                if (!value.Contains("ns=0;i="))
                {
                    value = value.Replace("ns=1;i=", "");
                    Reference _ref = new Reference
                    {
                        Value = "ns=1;i=" + value,
                        ReferenceType = type
                    };
                    return _ref;
                }
                else
                {
                    Reference _ref = new Reference
                    {
                        Value = value,
                        ReferenceType = type
                    };
                    return _ref;
                }

            }
            return null;
        }

        private static string CreateAASQualifier(string type, string value, string valueType, AdminShellV20.SemanticId semanticId, string dic)
        {
            UAObject qual = new UAObject();
            qual.NodeId = "ns=1;i=" + masterID.ToString();
            qual.BrowseName = "1:Qualifier:" + type + ":" + value;
            masterID++;
            List<Reference> refs = new List<Reference>();

            //map Qualifier Data
            refs.Add(CreateHasTypeDefinition("1:AASQualifierType"));
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/Qualifier"));
            refs.Add(CreateHasDictionaryEntry(dic));

            refs.Add(
                CreateReference("HasProperty", CreateProperty(type, "PropertyType", "Type", "String", "3:http://admin-shell.io/aas/2/0/Qualifier/type")));
            refs.Add(
                CreateReference(
                    "HasProperty", CreateProperty(value, "PropertyType", "Value", "BaseDataType", "3:http://admin-shell.io/aas/2/0/Qualifier/value")));
            refs.Add(
                CreateReference(
                    "HasProperty", CreateProperty(valueType, "PropertyType", "ValueType", "AASValueTypeDataType", "3:http://admin-shell.io/aas/2/0/Qualifier/valueType")));

            if (semanticId != null)
            {
                if (type == "IRI")
                {
                    refs.Add(CreateReference("HasDictionaryEntry", CreateIriConceptDescription(semanticId, "")));
                }
                else
                {
                    refs.Add(CreateReference("HasDictionaryEntry", CreateIrdiConceptDescription(semanticId, "")));
                }
            }

            qual.References = refs.ToArray();
            root.Add((UANode)qual);
            return qual.NodeId;
        }

        private static string CreateSubmodelElement(AdminShellV20.SubmodelElement element, string dic)
        {
            UAObject elem = new UAObject();
            elem.NodeId = "ns=1;i=" + masterID.ToString();
            masterID++;

            List<Reference> refs = new List<Reference>();

            if(!string.IsNullOrWhiteSpace(element.category)) refs.Add(CreateReference("HasProperty", CreateProperty(element.category, "PropertyType", "Category", "String", "")));

            refs.Add(CreateReference("HasComponent", CreateIriConceptDescription(element.semanticId, "")));
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/SubmodelElement"));
            refs.Add(CreateHasDictionaryEntry(dic));

            //add Referable &
            //add Description if it exists
            if (element.description == null)
            {
                refs.Add(CreateReference("HasInterface", CreateReferable(element.category, null, "")));
            }
            else
            {
                refs.Add(
                    CreateReference(
                        "HasInterface", CreateReferable(element.category, element.description.langString, "")));
            }

            //add Kind if it exists
            if (element.kind != null)
                //-> Anpassen
                refs.Add(CreateReference("HasProperty", CreateProperty(GetAASModelingKindDataType(element.kind.kind).ToString(), "PropertyType", "ModelingKind", "AASKindDataType", "3:http://admin-shell.io/aas/2/0/SubmodelElement/kind")));


            //add Qualifier if it exists
            if (element.qualifiers != null)
            {
                foreach (AdminShellV20.Qualifier qualifier in element.qualifiers)
                {
                    refs.Add(
                        CreateReference(
                            "HasComponent", CreateAASQualifier(qualifier.type, qualifier.value, qualifier.valueType, qualifier.semanticId, "3:http://admin-shell.io/aas/2/0/ SubmodelElement/qualifiers")));
                }
            }
           

            //Set Elementspecific Data
            string type = element.GetElementName();
            elem.BrowseName = "1:" + type + ":" + element.idShort;
            switch (type)
            {

                case "SubmodelElementCollection":
                    AdminShellV20.SubmodelElementCollection coll = (AdminShellV20.SubmodelElementCollection)element;
                    refs.Add(CreateHasTypeDefinition("1:AASSubmodelElementCollectionType"));
                    CreateSubmodelElementCollection(coll, refs);
                    break;

                case "Property":
                    AdminShellV20.Property prop = (AdminShellV20.Property)element;
                    refs.Add(CreateHasTypeDefinition("1:AASPropertyType"));
                    CreatePropertyType(prop.value, prop.valueId, prop.valueType, refs);
                    break;

                case "Operation":
                    AdminShellV20.Operation op = (AdminShellV20.Operation)element;
                    refs.Add(CreateHasTypeDefinition("1:AASOperationType"));
                    refs.Add(CreateReference("HasComponent", CreateAASOperation(op.inputVariable, op.outputVariable)));
                    break;

                case "Blob":
                    AdminShellV20.Blob blob = (AdminShellV20.Blob)element;
                    refs.Add(CreateHasTypeDefinition("1:AASBlobType"));
                    CreateAASBlob(blob.value, blob.mimeType, refs);
                    break;

                case "File":
                    AdminShellV20.File file = (AdminShellV20.File)element;
                    refs.Add(CreateHasTypeDefinition("1:AASFileType"));
                    CreateAASFile(file.value, file.mimeType, refs);
                    break;

                case "RelationshipElement":
                    AdminShellV20.RelationshipElement rela = (AdminShellV20.RelationshipElement)element;
                    refs.Add(CreateHasTypeDefinition("1:AASRelationshipElementType"));
                    CreateAASRelationshipElement(rela, refs);
                    break;

                case "ReferenceElement":
                    AdminShellV20.ReferenceElement refe = (AdminShellV20.ReferenceElement)element;
                    refs.Add(CreateHasTypeDefinition("1:AASReferenceType"));
                    CreateReferenceElement(refe, refs);
                    break;

                case "MultiLanguageProperty":
                    MultiLanguageProperty multi = (MultiLanguageProperty)element;
                    refs.Add(CreateHasTypeDefinition("1:AASMultiLanguagePropertyType"));
                    CreateMultiLanguageProperty(multi, refs);
                    break;

                case "Range":
                    Range range = (Range)element;
                    refs.Add(CreateHasTypeDefinition("1:AASRangeType"));
                    CreateRangeElement(range, refs);
                    break;

                case "Entity":
                    Entity ent = (Entity)element;
                    refs.Add(CreateHasTypeDefinition("1:AASEntityType"));
                    CreateEntityElement(ent, refs);
                    break;
            }

            elem.References = refs.ToArray();
            root.Add((UANode)elem);
            return elem.NodeId;
        }


        //SubmodelElementData Creation
        //always the same pattern -> mapping Data
        //

        private static void CreateSubmodelElementCollection(AdminShellV20.SubmodelElementCollection collection, List<Reference> refs)
        {
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/SubmodelElementCollection"));
            foreach (AdminShellV20.SubmodelElementWrapper elem in collection.value)
            {
                refs.Add(CreateReference("HasComponent", CreateSubmodelElement(elem.submodelElement, "3:http://admin-shell.io/aas/2/0/SubmodelElementCollection/values")));
            }
            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateProperty( collection.allowDuplicates.ToString().ToLower(), "PropertyType", "AllowDublication", "Boolean", "3:http://admin-shell.io/aas/2/0/ SubmodelElementCollection/allowDuplicates")));
        }

        private static void CreatePropertyType(string value, AdminShellV20.Reference valueId, string valueType, List<Reference> refs)
        {
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/Property"));
            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateProperty(value, "PropertyType", "Value", "BaseDataType", "3:http://admin-shell.io/aas/2/0/Property/value")));
            if (valueId != null)
                refs.Add(
                    CreateReference(
                        "HasComponent",
                        CreateKeyProperty(valueId.Keys, "ValueId", "3:http://admin-shell.io/aas/2/0/Property/valueId")));
            refs.Add(CreateReference("HasProperty", CreateProperty(GetAASValueTypeDataType(valueType).ToString(), "PropertyType", "ValueType", "AASValueTypeDataType", "3:http://admin-shell.io/aas/2/0/Property/valueType")));
        }

        private static void CreateMultiLanguageProperty(MultiLanguageProperty value, List<Reference> refs)
        {
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/MultiLanguageProperty"));
            refs.Add(CreateReference("HasProperty", CreateLocalizedTextProperty("Value", value.value.langString, "3:http://admin-shell.io/aas/2/0/ MultiLanguageProperty/value")));

            if (value.valueId != null)
                refs.Add(
                    CreateReference(
                        "HasComponent",
                        CreateKeyProperty(value.valueId.Keys, "ValueId", "3:http://admin-shell.io/aas/2/0/MultiLanguageProperty/valueId")));
        }

        private static void CreateAASCapability(string value, List<Reference> refs)
        {
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/Capability"));
            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateProperty(value, "BaseVariableType", "Capability", "String", "")));
        }

        private static string CreateAASOperation(List<AdminShellV20.OperationVariable> vin, List<AdminShellV20.OperationVariable> vout)
        {
            UAObject prop = new UAObject();
            prop.NodeId = "ns=1;i=" + masterID.ToString();
            prop.BrowseName = "1:Operation";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASOperationType"));



            prop.References = refs.ToArray();
            root.Add((UANode)prop);
            return prop.NodeId;
        }

        private static void CreateAASBlob(string value, string mimeType, List<Reference> refs)
        {
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/Blob"));
            refs.Add(CreateHasTypeDefinition("1:AASBlobType"));
            refs.Add(
                CreateReference(
                    "HasComponent",
                    CreateProperty(value, "FileType", "File", "String", "3:http://admin-shell.io/aas/2/0/Blob/value")));
            refs.Add(
                CreateReference(
                    "HasComponent",
                    CreateProperty(mimeType, "PropertyType", "MimeType", "String", "3:http://admin-shell.io/aas/2/0/Blob/mimeType")));
        }

        private static void CreateAASFile(string value, string mimeType, List<Reference> refs, string file = null)
        {
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/File"));
            refs.Add(CreateHasTypeDefinition("1:AASFileType"));
            refs.Add(CreateReference("HasProperty", CreateProperty(value, "PropertyType", "Value", "String", "3:http://admin-shell.io/aas/2/0/File/value")));
            refs.Add(CreateReference("HasProperty", CreateProperty(mimeType, "PropertyType", "MimeType", "String", "3:http://admin-shell.io/aas/2/0/File/mimeType")));
            if (file != null)
                refs.Add(CreateReference("HasComponent", CreateProperty(file, "FileType", "File", "String", "")));
        }

        private static void CreateAASRelationshipElement(RelationshipElement rela, List<Reference> refs)
        {
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/RelationshipElement"));
            refs.Add(CreateReference("HasComponent", CreateReference(rela.first, "First", "3:http://admin-shell.io/aas/2/0/RelationshipElement/first")));
            refs.Add(CreateReference("HasComponent", CreateReference(rela.second, "Second", "3:http://admin-shell.io/aas/2/0/RelationshipElement/second")));
        }

        private static void CreateRangeElement(Range range, List<Reference> refs)
        {
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/Range"));
            refs.Add(CreateReference("HasProperty", CreateProperty(GetAASValueTypeDataType(range.valueType).ToString(), "PropertyType", "ValueType", "AASValueTypeDataType", "3:http://admin-shell.io/aas/2/0/Range/valueType")));
            refs.Add(CreateReference("HasProperty", CreateProperty(range.min, "PropertyType", "Min", "BaseDataType", "3:http://admin-shell.io/aas/2/0/Range/min")));
            refs.Add(CreateReference("HasProperty", CreateProperty(range.max, "PropertyType", "Max", "BaseDataType", "3:http://admin-shell.io/aas/2/0/Range/max")));
        }

        private static void CreateReferenceElement(ReferenceElement value, List<Reference> refs)
        {
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/ReferenceElement"));
            refs.Add(CreateReference("HasProperty", CreateKeyProperty(value.value.Keys, "Value", "3:http://admin-shell.io/aas/2/0/ReferenceElement/value")));
        }

        private static void CreateEntityElement(Entity en, List<Reference> refs)
        {
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/Entity"));
            refs.Add(CreateReference("HasComponent", CreateReference(en.assetRef, "Asset", "3:http://admin-shell.io/aas/2/0/Entity/asset")));
            refs.Add(CreateReference("HasProperty", CreateProperty(GetAASEntityTypeDataType(en.entityType).ToString(), "PropertyType", "EntityType", "AASEntityTypeDataType", "3:http://admin-shell.io/aas/2/0/Entity/entityType")));
        }

        private static Reference CreateHasTypeDefinition(string type)
        {
            string _value = findBaseType(type);

            if (_value == null)
            {
                try
                {
                    _value = root.Find(x => x.BrowseName == type).NodeId;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return null;
                }
            }

            return CreateReference("HasTypeDefinition", _value);
        }

        private static string findBaseType(string type)
        {
            //because the BaseTypes are not in the mapping XML
            //they need to be somewhere, hard coded
            string value = null;
            switch (type)
            {
                case "BaseVariableType":
                    value = "ns=0;i=62";
                    break;

                case "BaseObjectType":
                    value = "ns=0;i=58";
                    break;

                case "PropertyType":
                    value = "ns=0;i=68";
                    break;

                case "DictionaryEntryType":
                    value = "ns=0;i=17589";
                    break;

                case "FileType":
                    value = "ns=0;i=11575";
                    break;

                case "BaseDataVariableType":
                    value = "ns=0;i=63";
                    break;

                case "String":
                    value = "ns=0;i=17589";
                    break;

                case "UriDictionaryEntryType":
                    value = "ns=0;i=17600";
                    break;
            }
            return value;
        }

        //Interface Creation

        private static string CreateIdentifiable(string id, string idtype, string version, string revision)
        {
            UAObject ident = new UAObject();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = "1:AASIdentifiable";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:IAASIdentifiableType"));
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/Identifiable"));

            //add Identification and Administration
            if (id != null || idtype != null)
                refs.Add(CreateReference("HasProperty", CreateIdentifiableIdentification(id, idtype, "3:http://admin-shell.io/aas/2/0/Identifiable/identification")));
            if (version != null || revision != null)
                refs.Add(CreateReference("HasProperty", CreateIdentifiableAdministration(version, revision, "3:http://admin-shell.io/aas/2/0/ Identifiable/administration")));

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        private static string CreateIdentifiableIdentification(string id, string idtype, string dic)
        {
            UAVariable ident = new UAVariable();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = "1:Identification";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASIdentifierType"));
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/Identifier"));
            refs.Add(CreateHasDictionaryEntry(dic));

            refs.Add(CreateReference("HasProperty", CreateProperty(id, "PropertyType", "Id", "String", "3:http://admin-shell.io/aas/2/0/Identifier/id")));
            refs.Add(
                CreateReference(
                    "HasProperty", CreateProperty(GetAASIdentifierTypeDataType(idtype).ToString(), "PropertyType", "IdType", "AASIdentifierTypeDataType", "3:http://admin-shell.io/aas/2/0/Identifier/idType")));

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        private static string CreateIdentifiableAdministration(string version, string revision, string dic)
        {
            UAVariable ident = new UAVariable();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = "1:Administration";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASAdministrativeInformationType"));
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/AdministrativeInformation"));
            refs.Add(CreateHasDictionaryEntry(dic));

            refs.Add(
                CreateReference(
                    "HasProperty", CreateProperty(version, "PropertyType", "Version", "String", "3:http://admin-shell.io/aas/2/0/AdministrativeInformation/version")));
            refs.Add(
                CreateReference(
                    "HasProperty", CreateProperty(revision, "PropertyType", "Revision", "String", "3:http://admin-shell.io/aas/2/0/AdministrativeInformation/version")));

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        private static string CreateReferable(string category, List<AdminShellV20.LangStr> langstr, string dic)
        {
            UAVariable ident = new UAVariable();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = "1:AASReferable";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:IAASReferableType"));
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/Referable"));
            refs.Add(CreateHasDictionaryEntry(dic));

            if (langstr != null)
            {
                refs.Add(CreateReference("HasProperty", CreateLocalizedTextProperty("Description", langstr, "")));
            }

            refs.Add(CreateReference("HasProperty", CreateProperty(GetAASCategoryDataType(category).ToString(), "PropertyType", "Category", "AASCategoryDataType", "3:http://admin-shell.io/aas/2/0/Referable/category")));

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        private static string CreateAASAsset(AdminShellV20.Asset asset, string dic)
        {
            UAVariable ident = new UAVariable();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = "1:" + "Asset:" + asset.idShort;
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASAssetType"));
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/Asset"));
            refs.Add(CreateHasDictionaryEntry(dic));

            if (asset.kind != null)
                //refs.Add(CreateReference( "HasProperty", CreateProperty(GetAASAssetKindDataType(asset.kind.kind).ToString(), "PropertyType", "AssetKind", "AASAssetKindDataType", "3:http://admin-shell.io/aas/2/0/Asset/assetKind")));

                //check if either administration or identification is null, 
                // -> (because if identification were null, you could not access id or idType)
                //then set accordingly
                //if (asset.administration == null)
                //{
                //    refs.Add(
                //        CreateReference(
                //            "HasComponent",
                //            CreateIdentifiable(asset.identification.id, asset.identification.idType, null, null)));
                //}
                //else if (asset.identification == null)
                //{
                //    refs.Add(
                //        CreateReference(
                //            "HasComponent",
                //            CreateIdentifiable(null, null, asset.administration.version, asset.administration.revision)));
                //}
                //else
                //{
                //    refs.Add(
                //        CreateReference(
                //            "HasComponent",
                //            CreateIdentifiable(
                //                asset.identification.id, asset.identification.idType, asset.administration.version,
                //                asset.administration.revision)));
                //}



                //if (asset.description == null)
                //{
                //    refs.Add(CreateReference("HasComponent", CreateReferable(asset.category, null, "")));
                //}
                //else
                //{
                //    refs.Add(
                //        CreateReference(
                //            "HasComponent", CreateReferable(asset.category, asset.description.langString)));
                //}
            if (asset.administration != null && asset.identification != null) refs.Add(CreateReference("HasInterface", CreateIdentifiable(asset.identification.id, asset.identification.idType, asset.administration.version, asset.administration.revision)));

            refs.Add(CreateReference("HasProperty", CreateProperty(asset.category, "PropertyType", "Category", "String", "")));
            if (asset.description != null && asset.description.langString != null)
            {
                refs.Add(CreateReference("HasProperty", CreateLocalizedTextProperty("Description", asset.description.langString, "")));
            }

            if (asset.assetIdentificationModelRef != null) refs.Add(CreateReference("HasComponent", CreateKeyProperty(asset.assetIdentificationModelRef.Keys, "AssetIdentificationModel", "3:http://admin-shell.io/aas/2/0/Asset/assetIdentificationModel")));
            if (asset.billOfMaterialRef != null) refs.Add(CreateReference("HasComponent", CreateKeyProperty(asset.billOfMaterialRef.Keys, "BillOfMaterial", "3:http://admin-shell.io/aas/2/0/Asset/billOfMaterial")));

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        //ConceptDictionary Creation

        private static string CreateConceptDictionaryFolder(List<AdminShellV20.ConceptDescription> concepts)
        {
            UAObject ident = new UAObject();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = "1:ConceptDictionary";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASConceptDictionaryType"));

            foreach (AdminShellV20.ConceptDescription con in concepts)
            {
                refs.Add(CreateReference("HasComponent", CreateDictionaryEntry(con)));
            }

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        private static string CreateDictionaryEntry(AdminShellV20.ConceptDescription concept)
        {
            UAObject entry = new UAObject();
            entry.NodeId = "ns=1;i=" + masterID.ToString();

            entry.BrowseName = "1:" + concept.ToCaptionInfo().Item2;


            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("DictionaryEntryType"));

            if (concept.identification.idType == "IRI")
            {
                refs.Add(CreateReference("HasComponent", CreateUriConceptDescription(concept)));
            }
            else
            {
                refs.Add(CreateReference("HasComponent", CreateIrdConceptDescription(concept)));
            }

            entry.References = refs.ToArray();
            root.Add((UANode)entry);
            return entry.NodeId;
        }

        private static string CreateUriConceptDescription(AdminShellV20.ConceptDescription concept)
        {
            UAObject ident = new UAObject();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = "1:AASIriConceptDescription";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASIriConceptDescriptionType"));

            refs.Add(CreateReference("HasComponent", CreateDataSpecification(concept)));
            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateIdentifiable(concept.identification.id, concept.identification.idType, null, null)));

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        private static string CreateIrdConceptDescription(AdminShellV20.ConceptDescription concept)
        {
            UAObject ident = new UAObject();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = "1:AASIrdiConceptDescription";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASIrdiConceptDescriptionType"));

            refs.Add(CreateReference("HasComponent", CreateDataSpecification(concept)));
            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateIdentifiable(concept.identification.id, concept.identification.idType, null, null)));

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        private static string CreateDataSpecification(AdminShellV20.ConceptDescription concept)
        {
            UAObject ident = new UAObject();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = "1:DataSpecification";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASDataSpecificationType"));

            refs.Add(CreateReference("HasComponent", CreateDataSpecificationIEC61360(concept)));

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        private static string CreateDataSpecificationIEC61360(AdminShellV20.ConceptDescription concept)
        {
            UAObject ident = new UAObject();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = "1:DataSpecificationIEC61360";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASDataSpecificationIEC61360Type"));
            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateProperty(
                        "DataSpecificationIEC61360", "PropertyType", "DefaultInstanceBrowseName", "String", "")));
            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateProperty("DataSpecificationIEC61360", "PropertyType", "IdShort", "String", "3:http://admin-shell.io/DataSpecification/idShort")));
            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateProperty(GetAASCategoryDataType(concept.category).ToString(), "PropertyType", "Category", "AASCategoryDataType", "3:http://admin-shell.io/DataSpecification/category")));

            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateProperty(concept.GetIEC61360().dataType, "BaseVariableType", "DataType", "String", "3:http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0/ DataSpecificationIEC61360/dataType")));

            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateLocalizedTextProperty("Definition", concept.GetIEC61360().definition.langString, "3:http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0/ DataSpecificationIEC61360/definition")));
            refs.Add(
                CreateReference(
                    "HasComponent",
                    CreateLocalizedTextProperty("PreferredName", concept.GetIEC61360().preferredName.langString, "3:http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0/ DataSpecificationIEC61360/preferredName")));

            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateProperty(
                        concept.GetIEC61360().shortName.ToString(), "BaseVariableType", "ShortName", "String", "3:http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0/ DataSpecificationIEC61360/shortName")));
            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateProperty(concept.GetIEC61360().symbol, "BaseVariableType", "Symbol", "String", "3:http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0/ DataSpecificationIEC61360/symbol")));
            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateProperty(concept.GetIEC61360().unit, "BaseVariableType", "Unit", "String", "3:http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0/ DataSpecificationIEC61360/unit")));
            refs.Add(
                CreateReference(
                    "HasProperty",
                    CreateProperty(concept.GetIEC61360().valueFormat, "BaseVariableType", "ValueFormat", "String", "3:http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0/ DataSpecificationIEC61360/valueFormat")));

            refs.Add( CreateReference( "HasProperty",
                    CreateProperty(concept.embeddedDataSpecification.dataSpecificationContent.dataSpecificationIEC61360.sourceOfDefinition, "BaseVariableType", "SourceOfDestination", "String", "3:http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0/ DataSpecificationIEC61360/sourceOfDefinition")));

            refs.Add(CreateReference("HasProperty",
                    CreateProperty(concept.embeddedDataSpecification.dataSpecificationContent.dataSpecificationIEC61360.sourceOfDefinition, "BaseVariableType", "SourceOfDestination", "String", "3:http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0/ DataSpecificationIEC61360/sourceOfDefinition")));

            if(concept.embeddedDataSpecification.dataSpecificationContent.dataSpecificationIEC61360.unitId != null) refs.Add(CreateReference("HasComponent", CreateKeyProperty(concept.embeddedDataSpecification.dataSpecificationContent.dataSpecificationIEC61360.unitId.Keys, "UnitId", "3:http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0/ DataSpecificationIEC61360/unitId")));

            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/2/0"));

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        //Asset Creation

        private static string CreateView(AdminShellV20.View view, string dic)
        {
            UAVariable var = new UAVariable();
            var.NodeId = "ns=1;i=" + masterID.ToString();
            var.BrowseName = "1:" + "View:" + view.idShort;
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASViewType"));
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/View"));
            refs.Add(CreateHasDictionaryEntry(dic));

            if (view.description != null)
            {
                refs.Add(CreateReference("HasComponent", CreateReferable(view.category, view.description.langString, "")));
            }

            foreach (AdminShellV20.ContainedElementRef con in view.containedElements.reference)
            {
                refs.Add(CreateReference("HasComponent", createContainedElement(con)));
            }


            var.References = refs.ToArray();
            root.Add((UANode)var);
            return var.NodeId;
        }

        private static string CreateDerivedFrom(List<AdminShellV20.Key> keys)
        {
            UAObject obj = new UAObject();
            obj.NodeId = "ns=1;i=" + masterID.ToString();
            obj.BrowseName = "1:DerivedFrom";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASReferenceType"));

            foreach (AdminShellV20.Key key in keys)
            {
                refs.Add(
                    CreateReference(
                        "HasProperty", CreateKeyProperty(key, "Key", "")));
            }

            obj.References = refs.ToArray();
            root.Add((UANode)obj);
            return obj.NodeId;
        }

        private static string CreateHasDataSpecification(AdminShellV20.HasDataSpecification data)
        {
            UAObject obj = new UAObject();
            obj.NodeId = "ns=1;i=" + masterID.ToString();
            obj.BrowseName = "1:DataSpecification";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASReferenceType"));

            foreach (AdminShellV20.Reference _ref in data.reference)
            {
                refs.Add(CreateReference("HasComponent", CreateReference(_ref, "Reference", "")));
            }

            obj.References = refs.ToArray();
            root.Add((UANode)obj);
            return obj.NodeId;
        }

        private static string CreateReference(AdminShellV20.Reference _ref, string name, string dic)
        {
            UAObject obj = new UAObject();
            obj.NodeId = "ns=1;i=" + masterID.ToString();
            obj.BrowseName = "1:" + name;
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASReferenceType"));
            refs.Add(CreateHasDictionaryEntry("3:http://admin-shell.io/aas/2/0/Reference"));
            refs.Add(CreateHasDictionaryEntry(dic));

            foreach (AdminShellV20.Key key in _ref.Keys)
            {
                refs.Add(
                    CreateReference(
                        "HasComponent", CreateKeyProperty(key, "Key", "3:http://admin-shell.io/aas/2/0/Reference/keys")));
            }

            obj.References = refs.ToArray();
            root.Add((UANode)obj);
            return obj.NodeId;
        }

        private static string CreateAssetRef(AdminShellV20.AssetRef ass, string dic)
        {
            UAObject obj = new UAObject();
            obj.NodeId = "ns=1;i=" + masterID.ToString();
            obj.BrowseName = "1:AssetRef";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASReferenceType"));
            refs.Add(CreateHasDictionaryEntry(dic));

            foreach (AdminShellV20.Key key in ass.Keys)
            {
                refs.Add(
                    CreateReference(
                        "HasComponent", CreateKeyProperty(key, "Key", "")));
            }

            obj.References = refs.ToArray();
            root.Add((UANode)obj);
            return obj.NodeId;
        }

        private static string createContainedElement(AdminShellV20.ContainedElementRef ele)
        {
            UAVariable var = new UAVariable();
            var.NodeId = "ns=1;i=" + masterID.ToString();
            var.BrowseName = "1:ContainedElementRef";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASReferenceType"));

            foreach (AdminShellV20.Key key in ele.Keys)
            {
                refs.Add(
                    CreateReference(
                        "HasComponent", CreateKeyProperty(key, "Key", "")));
            }

            var.References = refs.ToArray();
            root.Add((UANode)var);
            return var.NodeId;
        }


        private static string CreateIriConceptDescription(SemanticId sem, string dic)
        {
            if (sem == null)
                return null;

            UAObject ident = new UAObject();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = "1:AASIriConceptDescription";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASIriConceptDescriptionType"));
            refs.Add(CreateHasDictionaryEntry(dic));

            refs.Add(CreateReference("IsCaseOf", CreateConceptDescription(sem.Keys, "")));

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        private static string CreateIrdiConceptDescription(SemanticId sem, string dic)
        {
            UAObject ident = new UAObject();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = "1:AASIrdiConceptDescription";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASIrdiConceptDescriptionType"));
            refs.Add(CreateHasDictionaryEntry(dic));

            refs.Add(CreateReference("IsCaseOf", CreateConceptDescription(sem.Keys, "")));

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        private static string CreateConceptDescription(List<Key> keys, string dic)
        {
            UAObject ident = new UAObject();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = "1:ConceptDescription";
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASReferenceType"));
            refs.Add(CreateHasDictionaryEntry(dic));

            //refs.Add(CreateReference("HasComponent", CreateKeyContainer(keys, "Keys")));
            refs.Add(CreateReference("HasComponent", CreateKeyProperty(keys, "Keys", "")));

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }



        private static XmlElement CreateLocalizedTextValue(List<LangStr> strs)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement( "uax", "ListOfLocalizedText", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            
            foreach(LangStr str in strs)
            {
                XmlNode node = doc.CreateNode(XmlNodeType.Element, "uax", "LocalizedText", "http://opcfoundation.org/UA/2008/02/Types.xsd");
                XmlNode locale = doc.CreateNode(XmlNodeType.Element, "uax", "Locale", "http://opcfoundation.org/UA/2008/02/Types.xsd");
                locale.InnerText = str.lang;
                XmlNode text = doc.CreateNode(XmlNodeType.Element, "uax", "Text", "http://opcfoundation.org/UA/2008/02/Types.xsd");
                text.InnerText = str.str;

                node.AppendChild(locale);
                node.AppendChild(text);
                element.AppendChild(node);
            }

            return element;
        }

        private static string CreateLocalizedTextProperty(string name, List<LangStr> list, string dic)
        {
            UAVariable prop = new UAVariable();
            prop.NodeId = "ns=1;i=" + masterID.ToString();
            prop.BrowseName = "1:" + name;
            prop.DataType = "LocalizedText";
            masterID++;

            prop.Value = CreateLocalizedTextValue(list);

            prop.References = new Reference[2];
            prop.References[0] = CreateHasTypeDefinition("PropertyType");
            prop.References[1] = CreateHasDictionaryEntry(dic);

            prop.ValueRank = 1;
            prop.ArrayDimensions = list.Count.ToString();

            root.Add((UANode)prop);
            return prop.NodeId;
        }

        private static XmlNode CreateKeyValue(Key key, XmlDocument doc)
        {
            XmlNode root = doc.CreateNode(XmlNodeType.Element, "uax", "ExtensionObject", "http://opcfoundation.org/UA/2008/02/Types.xsd");

            XmlNode typeId = doc.CreateNode(XmlNodeType.Element, "uax", "TypeId", "http://opcfoundation.org/UA/2008/02/Types.xsd");

            XmlNode identifier = doc.CreateNode(XmlNodeType.Element, "uax", "Identifier", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            identifier.InnerText = "ns=1;i=5039";
            typeId.AppendChild(identifier);
            root.AppendChild(typeId);

            XmlNode body = doc.CreateNode(XmlNodeType.Element, "uax", "Body", "http://opcfoundation.org/UA/2008/02/Types.xsd");
            XmlNode xmlKey = doc.CreateNode(XmlNodeType.Element, "", "AASKeyDataType", typesNS);

            XmlNode type = doc.CreateNode(XmlNodeType.Element, "", "Type", "");
            type.InnerText = GetAASKeyTypeDataType(key.idType).ToString();
            xmlKey.AppendChild(type);

            XmlNode local = doc.CreateNode(XmlNodeType.Element, "", "Local", "");
            local.InnerText = key.local.ToString();
            xmlKey.AppendChild(local);

            XmlNode value = doc.CreateNode(XmlNodeType.Element, "", "Value", "");
            value.InnerText = key.value.ToString();
            xmlKey.AppendChild(value);

            XmlNode idtype = doc.CreateNode(XmlNodeType.Element, "", "IdType", "");
            idtype.InnerText = GetAASKeyElementsDataType(key.type).ToString();
            xmlKey.AppendChild(idtype);

            body.AppendChild(xmlKey);
            root.AppendChild(body);

            return root;
        }

        private static XmlElement CreateKeysValue(List<Key> keys)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = doc.CreateElement("uax", "ListOfExtensionObject", "http://opcfoundation.org/UA/2008/02/Types.xsd");

            foreach(Key key in keys)
            {
                element.AppendChild(CreateKeyValue(key, doc));
            }

            return element;
        }

        private static string CreateKeyProperty(List<Key> keys, string name, string dic)
        {
            UAVariable ident = new UAVariable();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = name;
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASKeyTypeDataType"));
            refs.Add(CreateHasDictionaryEntry(dic));

            ident.Value = CreateKeysValue(keys);

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        private static string CreateKeyProperty(Key key, string name, string dic)
        {
            UAVariable ident = new UAVariable();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = name;
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("1:AASKeyTypeDataType"));
            refs.Add(CreateHasDictionaryEntry(dic));

            ident.Value = (XmlElement)CreateKeyValue(key, new XmlDocument());

            ident.References = refs.ToArray();
            root.Add((UANode)ident);
            return ident.NodeId;
        }

        private static Reference CreateHasDictionaryEntry(string name)
        {
            UAObject ident = new UAObject();
            ident.NodeId = "ns=1;i=" + masterID.ToString();
            ident.BrowseName = name;
            masterID++;
            List<Reference> refs = new List<Reference>();
            refs.Add(CreateHasTypeDefinition("UriDictionaryEntryType"));

            ident.References = refs.ToArray();
            root.Add((UANode)ident);

            return CreateReference("HasDictionaryEntry", ident.NodeId);

        }


        //Methods for the numerous Enumerations of i4AAS

        private static int GetAASKeyTypeDataType(string str)
        {
            int i = -1;
            switch (str)
            {
                case "IdShort":
                    i = 0;
                    break;
                case "FragmentId":
                    i = 1;
                    break;
                case "Custom":
                    i = 2;
                    break;
                case "IRDI":
                    i = 3;
                    break;
                case "IRI":
                    i = 4;
                    break;
            }
            return i;
        }

        private static int GetAASKeyElementsDataType(string str)
        {
            int i = -1;
            switch (str)
            {
                case "AccessPermissionRule":
                    i = 0;
                    break;
                case "AnnotatedRelationshipElement":
                    i = 1;
                    break;
                case "Asset":
                    i = 2;
                    break;
                case "AssetAdministrationShell":
                    i = 3;
                    break;
                case "Blob":
                    i = 4;
                    break;
                case "Capability":
                    i = 5;
                    break;
                case "ConceptDescription":
                    i = 6;
                    break;
                case "ConceptDictionary":
                    i = 7;
                    break;
                case "DataElement":
                    i = 8;
                    break;
                case "Entity":
                    i = 9;
                    break;
                case "Event":
                    i = 10;
                    break;
                case "File":
                    i = 11;
                    break;
                case "FragmentReference":
                    i = 12;
                    break;
                case "GlobalReference":
                    i = 13;
                    break;
                case "MultiLanguageProperty":
                    i = 14;
                    break;
                case "Property":
                    i = 15;
                    break;
                case "Operation":
                    i = 16;
                    break;
                case "Range":
                    i = 17;
                    break;
                case "ReferenceElement":
                    i = 18;
                    break;
                case "RelationshipElement":
                    i = 19;
                    break;
                case "Submodel":
                    i = 20;
                    break;
                case "SubmodelElement":
                    i = 21;
                    break;
                case "SubmodelElementCollection":
                    i = 22;
                    break;
                case "View":
                    i = 23;
                    break;
            }
            return i;
        }

        private static int GetAASIdentifierTypeDataType(string str)
        {
            int i = -1;
            switch (str)
            {
                case "IRDI":
                    i = 0;
                    break;
                case "IRI":
                    i = 1;
                    break;
                case "Custom":
                    i = 2;
                    break;
            }
            return i;
        }

        private static int GetAASAssetKindDataType(string str)
        {
            int i = -1;
            switch (str)
            {
                case "Type":
                    i = 0;
                    break;
                case "Instance":
                    i = 1;
                    break;
            }
            return i;
        }

        private static int GetAASModelingKindDataType(string str)
        {
            int i = -1;
            switch (str)
            {
                case "Template":
                    i = 0;
                    break;
                case "Instance":
                    i = 1;
                    break;
            }
            return i;
        }

        private static int GetAASValueTypeDataType(string str)
        {
            int i = -1;
            switch (str)
            {
                case "Boolean":
                    i = 0;
                    break;
                case "SByte":
                    i = 1;
                    break;
                case "Byte":
                    i = 2;
                    break;
                case "Int16":
                    i = 3;
                    break;
                case "UInt16":
                    i = 4;
                    break;
                case "Int32":
                    i = 5;
                    break;
                case "UInt32":
                    i = 6;
                    break;
                case "Int64":
                    i = 7;
                    break;
                case "UInt64":
                    i = 8;
                    break;
                case "Float":
                    i = 9;
                    break;
                case "Double":
                    i = 10;
                    break;
                case "String":
                    i = 11;
                    break;
                case "DateTime":
                    i = 12;
                    break;
                case "ByteString":
                    i = 14;
                    break;
                case "LocalizedText":
                    i = 20;
                    break;
                case "UtcTime":
                    i = 37;
                    break;
            }
            return i;
        }

        private static int GetAASEntityTypeDataType(string str)
        {
            int i = -1;
            switch (str)
            {
                case "CoManagedEntity":
                    i = 1;
                    break;
                case "SelfManagedEntity":
                    i = 2;
                    break;
            }
            return i;
        }

        private static int GetAASCategoryDataType(string str)
        {
            if(str != null) str = str.ToUpper();
            int i = -1;
            switch (str)
            {
                case "CONSTANT":
                    i = 0;
                    break;
                case "PARAMETER":
                    i = 1;
                    break;
                case "VARIABLE":
                    i = 2;
                    break;
                case "RELATIONSHIP":
                    i = 3;
                    break;
            }
            return i+1;
        }


    }
}
