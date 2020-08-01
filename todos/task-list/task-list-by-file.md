## AasxAmlImExport\AmlExport.cs

[Line 301, column 21](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxAmlImExport/AmlExport.cs#L301
), 
Michael Hoffmeister,
1970-01-01

    here is no equivalence to set a match!!

[Line 862, column 13](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxAmlImExport/AmlExport.cs#L862
), 
Michael Hoffmeister,
1970-01-01

    source99999

[Line 1041, column 9](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxAmlImExport/AmlExport.cs#L1041
), 
Michael Hoffmeister,
1970-01-01

    Export tasks
    * ConceptDescriptions - done
    * Views
    * Types -> SystemUnitClasses & linkings - done

## AasxAmlImExport\AmlImport.cs

[Line 191, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxAmlImExport/AmlImport.cs#L191
), 
MICHA+M. WIEGAND,
1970-01-01

    I dont understand the determinism behind that!
    WIEGAND: me, neither ;-)
    Wiegand:  ich hab mir von Prof.Drath nochmal erklären lassen, wie SupportedRoleClass und
    RoleRequirement verwendet werden:
    In CAEX2.15(aktuelle AML Version und unsere AAS Mapping Version):
      1.Eine SystemUnitClass hat eine oder mehrere SupportedRoleClasses, die ihre „mögliche Rolle
        beschreiben(Drucker / Fax / kopierer)
      2.Wird die SystemUnitClass als InternalElement instanziiert entscheidet man sich für eine
        Hauptrolle, die dann zum RoleRequirement wird und evtl. Nebenklassen die dann
        SupportedRoleClasses sind(ist ein Workaround weil CAEX2.15 in der Norm nur
        ein RoleReuqirement erlaubt)
    InCAEX3.0(nächste AMl Version):
      1.Wie bei CAEX2.15
      2.Wird die SystemUnitClass als Internal Elementinstanziiert werden die verwendeten Rollen
        jeweils als RoleRequirement zugewiesen (in CAEX3 sind mehrere RoleReuqirements nun erlaubt)

[Line 1343, column 45](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxAmlImExport/AmlImport.cs#L1343
), 
Michael Hoffmeister,
1970-01-01

    fill out 
    eds.hasDataSpecification by using outer attributes

## AasxCsharpLibrary\AdminShell.cs

[Line 2578, column 13](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxCsharpLibrary/AdminShell.cs#L2578
), 
Michael Hoffmeister,
1970-01-01

    in V1.0, shall be a list of embeddedDataSpecification

[Line 3528, column 13](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxCsharpLibrary/AdminShell.cs#L3528
), 
Michael Hoffmeister,
1970-01-01

    check, if Json has Qualifiers or not

[Line 4245, column 21](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxCsharpLibrary/AdminShell.cs#L4245
), 
MIHO,
2020-07-31

    would be nice to use IEnumerateChildren for this ..

## AasxCsharpLibrary\AdminShellPackageEnv.cs

[Line 230, column 25](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxCsharpLibrary/AdminShellPackageEnv.cs#L230
), 
Michael Hoffmeister,
1970-01-01

    use aasenv serialzers here!

[Line 350, column 21](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxCsharpLibrary/AdminShellPackageEnv.cs#L350
), 
Michael Hoffmeister,
1970-01-01

    use aasenv serialzers here!

[Line 389, column 25](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxCsharpLibrary/AdminShellPackageEnv.cs#L389
), 
Michael Hoffmeister,
1970-01-01

    use aasenv serialzers here!

[Line 413, column 25](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxCsharpLibrary/AdminShellPackageEnv.cs#L413
), 
Michael Hoffmeister,
1970-01-01

    use aasenv serialzers here!

## AasxPredefinedConcepts\Convert\ConvertDocumentationSg2ToHsu.cs

[Line 33, column 13](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxPredefinedConcepts/Convert/ConvertDocumentationSg2ToHsu.cs#L33
), 
MIHO,
2020-07-31

    temporary have code to allow conversion of Festo MCAD / ECAD models as well

[Line 60, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxPredefinedConcepts/Convert/ConvertDocumentationSg2ToHsu.cs#L60
), 
MIHO,
2020-07-31

    disable line above to allow more models, such as MCAD/ECAD

## AasxRestServerLibrary\AasxRestClient.cs

[Line 56, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxRestServerLibrary/AasxRestClient.cs#L56
), 
Michael Hoffmeister,
1970-01-01

    Andreas, check this. 
    System.Net.WebProxy.GetDefaultProxy was deprecated.

## AasxSignature\AasxSignature.cs

[Line 173, column 21](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxSignature/AasxSignature.cs#L173
), 
Michael Hoffmeister,
1970-01-01

    Is package according to the Logical model of the AAS?

[Line 207, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxSignature/AasxSignature.cs#L207
), 
Michael Hoffmeister,
1970-01-01

    is package sealed? => no other signatures can be added?

[Line 210, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxSignature/AasxSignature.cs#L210
), 
Michael Hoffmeister,
1970-01-01

    The information from the analysis
    -> return as an object (list of enums with the issues/warings???)

## AasxUANodesetImExport\UANodeSet.cs

[Line 20, column 1](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxUANodesetImExport/UANodeSet.cs#L20
), 
Michael Hoffmeister,
1970-01-01

    License

[Line 21, column 1](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxUANodesetImExport/UANodeSet.cs#L21
), 
Michael Hoffmeister,
1970-01-01

    Fraunhofer IOSB: Check ReSharper

## AasxUANodesetImExport\UANodeSetExport.cs

[Line 26, column 1](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxUANodesetImExport/UANodeSetExport.cs#L26
), 
Michael Hoffmeister,
1970-01-01

    License

[Line 27, column 1](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxUANodesetImExport/UANodeSetExport.cs#L27
), 
Michael Hoffmeister,
1970-01-01

    Fraunhofer IOSB: Check ReSharper

## AasxUANodesetImExport\UANodeSetImport.cs

[Line 23, column 1](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxUANodesetImExport/UANodeSetImport.cs#L23
), 
Michael Hoffmeister,
1970-01-01

    License

[Line 24, column 1](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxUANodesetImExport/UANodeSetImport.cs#L24
), 
Michael Hoffmeister,
1970-01-01

    Fraunhofer IOSB: Check ReSharper

## AasxWpfControlLibrary\DiplayVisualAasxElements.xaml.cs

[Line 610, column 21](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxWpfControlLibrary/DiplayVisualAasxElements.xaml.cs#L610
), 
MIHO,
2020-07-21

    was because of multi-select

## AasxWpfControlLibrary\DispEditAasxEntity.xaml.cs

[Line 2375, column 25](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxWpfControlLibrary/DispEditAasxEntity.xaml.cs#L2375
), 
Michael Hoffmeister,
1970-01-01

    add Sync to shortName

[Line 2863, column 37](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxWpfControlLibrary/DispEditAasxEntity.xaml.cs#L2863
), 
Michael Hoffmeister,
1970-01-01

    Operation mssing here?

[Line 2885, column 37](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxWpfControlLibrary/DispEditAasxEntity.xaml.cs#L2885
), 
Michael Hoffmeister,
1970-01-01

    Operation mssing here?

[Line 4024, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxWpfControlLibrary/DispEditAasxEntity.xaml.cs#L4024
), 
Michael Hoffmeister,
1970-01-01

    ordered, allowDuplicates

## AasxWpfControlLibrary\DispEditHelper.cs

[Line 1161, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxWpfControlLibrary/DispEditHelper.cs#L1161
), 
Michael Hoffmeister,
1970-01-01

    possibly [Jump] button??

[Line 1304, column 25](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxWpfControlLibrary/DispEditHelper.cs#L1304
), 
Michael Hoffmeister,
1970-01-01

    Needs to be revisited

## AasxWpfControlLibrary\VisualAasxElements.cs

[Line 153, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxWpfControlLibrary/VisualAasxElements.cs#L153
), 
MIHO,
2020-07-31

    check if commented out because of non-working multi-select?


