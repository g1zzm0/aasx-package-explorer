## AasxAmlImExport\AmlExport.cs

[Line 862, column 13](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxAmlImExport/AmlExport.cs#L862
), 
Michael Hoffmeister,
2020-08-01

    If further data specifications exist (in future), add here

## AasxAmlImExport\AmlImport.cs

[Line 177, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxAmlImExport/AmlImport.cs#L177
), 
MIHO,
2020-08-01

    The check for role class or requirements is still questionable
    but seems to be correct (see below)
    
    Question MIHO: I dont understand the determinism behind that!
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

[Line 1332, column 45](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxAmlImExport/AmlImport.cs#L1332
), 
Michael Hoffmeister,
2020-08-01

    fill out 
    eds.hasDataSpecification by using outer attributes

## AasxCsharpLibrary\AdminShell.cs

[Line 3527, column 13](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxCsharpLibrary/AdminShell.cs#L3527
), 
Michael Hoffmeister,
2020-08-01

    check, if Json has Qualifiers or not

[Line 4244, column 21](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxCsharpLibrary/AdminShell.cs#L4244
), 
MIHO,
2020-07-31

    would be nice to use IEnumerateChildren for this ..

## AasxCsharpLibrary\AdminShellPackageEnv.cs

[Line 230, column 25](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxCsharpLibrary/AdminShellPackageEnv.cs#L230
), 
Michael Hoffmeister,
2020-08-01

    use a unified function to create a serializer

[Line 350, column 21](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxCsharpLibrary/AdminShellPackageEnv.cs#L350
), 
Michael Hoffmeister,
2020-08-01

    use a unified function to create a serializer

[Line 389, column 25](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxCsharpLibrary/AdminShellPackageEnv.cs#L389
), 
Michael Hoffmeister,
2020-08-01

    use a unified function to create a serializer

[Line 413, column 25](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxCsharpLibrary/AdminShellPackageEnv.cs#L413
), 
Michael Hoffmeister,
2020-08-01

    use a unified function to create a serializer

## AasxRestServerLibrary\AasxRestClient.cs

[Line 56, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxRestServerLibrary/AasxRestClient.cs#L56
), 
Michael Hoffmeister,
2020-08-01

    Andreas, check this. 
    System.Net.WebProxy.GetDefaultProxy was deprecated.

## AasxSignature\AasxSignature.cs

[Line 25, column 9](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxSignature/AasxSignature.cs#L25
), 
Andreas Orzelski,
2020-08-01

    The signature file and [Content_Types].xml can be tampered?

[Line 175, column 21](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxSignature/AasxSignature.cs#L175
), 
Andreas Orzelski,
2020-08-01

    Is package according to the Logical model of the AAS?

[Line 209, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxSignature/AasxSignature.cs#L209
), 
Andreas Orzelski,
2020-08-01

    is package sealed? => no other signatures can be added?

[Line 212, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxSignature/AasxSignature.cs#L212
), 
Andreas Orzelski,
2020-08-01

    The information from the analysis
    -> return as an object (list of enums with the issues/warings???)

## AasxUANodesetImExport\UANodeSet.cs

[Line 20, column 1](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxUANodesetImExport/UANodeSet.cs#L20
), 
Michael Hoffmeister,
2020-08-01

    Fraunhofer IOSB: Check ReSharper to be OK

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
2020-08-01

    Fraunhofer IOSB: Check ReSharper settings to be OK

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
2020-08-01

    add Sync to shortName

[Line 2863, column 37](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxWpfControlLibrary/DispEditAasxEntity.xaml.cs#L2863
), 
Michael Hoffmeister,
2020-08-01

    Operation mssing here?

[Line 2885, column 37](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxWpfControlLibrary/DispEditAasxEntity.xaml.cs#L2885
), 
Michael Hoffmeister,
2020-08-01

    Operation mssing here?

## AasxWpfControlLibrary\DispEditHelper.cs

[Line 1161, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxWpfControlLibrary/DispEditHelper.cs#L1161
), 
Michael Hoffmeister,
2020-08-01

    possibly [Jump] button??

[Line 1304, column 25](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxWpfControlLibrary/DispEditHelper.cs#L1304
), 
Michael Hoffmeister,
2020-08-01

    Needs to be revisited

## AasxWpfControlLibrary\VisualAasxElements.cs

[Line 153, column 17](
https://github.com/admin-shell-io/aasx-package-explorer/blob/master/src/AasxWpfControlLibrary/VisualAasxElements.cs#L153
), 
MIHO,
2020-07-31

    check if commented out because of non-working multi-select?


