# How to add a new DataModel / Controller

## DataModel

Define new DataModel.
There are a few constraints. The Datamodel has to implement the Interfaces
IIdentifiable, 
IImportDateassigneable, 
IMetaData

optional
IMappingAware
IHasLanguage
ISource
IActivateable
IPublishedOn
ILicenseInfo

means the minimum Set of fields are:
Id
_Meta
FirstImport, LastChange
PublishedOn
Active
Mapping
Source
HasLanguage
LicenseInfo

Reuse Objects in your DataModel

## Controller

Define a Controller which inherits from OdhController
implement  
GET LIST
GET DETAIL
POST
PUT
DELETE
Methods

## Helpers
MetaInfo Helper
ODHTypeHelper
LicenseInfoHelper
