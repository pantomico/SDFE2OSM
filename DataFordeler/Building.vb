Public Class Building

    Public Enum Tagdækningsmateriale
        TagpapMedLilleHældning = 1
        TagpapMedStorHældning = 2
        FibercementHerunderAsbest = 3
        Betontagsten = 4
        Tegl = 5
        Metal = 6
        Stråtag = 7
        FibercementUdenAsbest = 10
        Plastmaterialer = 11
        Glas = 12
        LevendeTage = 20
        Ingen = 80
        Andet = 90
    End Enum
    Public Enum YdervaeggenesMateriale
        Mursten = 1
        Letbetonsten = 2
        FibercementHerunderAsbest = 3
        Bindingsværk = 4
        Træ = 5
        Betonelementer = 6
        Metal = 8
        FibercementUdenAsbest = 9
        Plastmaterialer = 11
        Glas = 12
        Ingen = 80
        Andet = 90
    End Enum
    ''' <summary>
    ''' Internally referenced as "status"
    ''' </summary>
    Public Enum LivsCyclus
        Started = 1
        Projekteret = 2
        UnderOpførsel = 3
        Sagsgrund = 4
        Oprettet = 5
        Opført = 6
        Gældende = 7
        Godkendt = 8
        Afsluttet = 9
        Historisk = 10
        Fejlregistreret = 11
        MidlertidigAfsluttet = 12
        DelvisAfsluttet = 13
        Henlagt = 14
        Modtaget = 15
        UnderBehandling = 16
        Fejl = 17
        Udført = 18
        Foreløbig = 19
    End Enum
    Public Enum Etagetype
        Normal = 0
        Roof = 1
        Basement = 2
    End Enum

    Public Structure FormattedBuilding
        Public Bygningsnummer As Integer
        Public Husnummer As String
        Public Anvendelse As BygningAnvendelse
        Public Opførelsesår As Integer
        Public OSMbuilding As String
        Public Tagdækningsmateriale As Tagdækningsmateriale
        Public YdervæggensMateriale As YdervaeggenesMateriale
        Public Areal As Integer
        Public Levels As Integer
        Public LevelsInRoof As Integer
        Public OSMroofMaterial As String
        Public OSMbuildingMaterial As String
        Public byg133KildeTilKoordinatsæt As String
        Public byg134KvalitetAfKoordinatsæt As String
        Public byg135SupplerendeOplysningOmKoordinatsæt As String
        Public byg136PlaceringPåSøterritorie As String
        Public byg404Koordinat As String
        Public byg406Koordinatsystem As String
        Public IsStandingNow As Boolean
        Public OSMCoordinate As PointF
        Public LivsCyclus As LivsCyclus
        Public ValidityDate As Date
    End Structure
    Shared Function FormatBuilding(Building As Building) As FormattedBuilding
        Dim f As New FormattedBuilding
        Dim i As Integer
        Try
            Dim st As LivsCyclus = Val(Building.status)
            Dim et As Etagetype
            Select Case st
                Case = LivsCyclus.Opført, LivsCyclus.Godkendt, LivsCyclus.UnderOpførsel
                    f.IsStandingNow = True
                Case Else
                    Debug.WriteLine($"Building found with status {[Enum].GetName(st)}")
                    f.IsStandingNow = False
            End Select
            f.LivsCyclus = st
            f.Bygningsnummer = Val(Building.byg007Bygningsnummer)
            f.Husnummer = Building.husnummer.Trim
            f.Anvendelse = Val(Building.byg021BygningensAnvendelse)
            f.Opførelsesår = Val(Building.byg026Opførelsesår)
            f.OSMbuilding = AnvændelseToOSMBuilding(Val(Building.byg021BygningensAnvendelse))
            f.Tagdækningsmateriale = Val(Building.byg033Tagdækningsmateriale)
            f.YdervæggensMateriale = Val(Building.byg032YdervæggensMateriale)
            f.Areal = Val(Building.byg038SamletBygningsareal)
            i = Val(Building.byg041BebyggetAreal)
            If i > f.Areal Then f.Areal = i

            f.Levels = Val(Building.byg054AntalEtager)
            If f.Levels = 0 Then f.Levels = 1
            f.LevelsInRoof = 0
            If Not (Building.etageList Is Nothing) Then
                For Each eta In Building.etageList
                    et = Val(eta.etage.eta025Etagetype)
                    If et = Etagetype.Roof Then f.LevelsInRoof += 1
                Next
            End If

            f.OSMroofMaterial = OSMroofMaterial(f.Tagdækningsmateriale)
            f.OSMbuildingMaterial = OSMWallMaterial(f.YdervæggensMateriale)
            f.byg133KildeTilKoordinatsæt = Building.byg133KildeTilKoordinatsæt
            f.byg134KvalitetAfKoordinatsæt = Building.byg134KvalitetAfKoordinatsæt
            f.byg135SupplerendeOplysningOmKoordinatsæt = Building.byg135SupplerendeOplysningOmKoordinatsæt
            f.byg404Koordinat = Building.byg404Koordinat
            f.byg406Koordinatsystem = Building.byg406Koordinatsystem
            f.ValidityDate = Building.byg094Revisionsdato
            If Building.virkningFra > f.ValidityDate Then f.ValidityDate = Building.virkningFra
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try

        Return f
    End Function

    <Serializable> Public Class Buildings
        Public Property Property1() As Building
    End Class

    <Serializable> Public Class Building
        Public Property datafordelerOpdateringstid As Date
        Public Property byg007Bygningsnummer As Integer
        Public Property byg021BygningensAnvendelse As String
        Public Property byg026Opførelsesår As Integer
        Public Property byg032YdervæggensMateriale As String
        Public Property byg033Tagdækningsmateriale As String
        'Public Property byg037KildeTilBygningensMaterialer As String
        Public Property byg038SamletBygningsareal As Integer
        Public Property byg039BygningensSamledeBoligAreal As Integer
        Public Property byg041BebyggetAreal As Integer
        Public Property byg044ArealIndbyggetUdhus As Integer
        Public Property byg053BygningsarealerKilde As String
        Public Property byg054AntalEtager As Integer
        'Public Property byg056Varmeinstallation As String
        'Public Property byg057Opvarmningsmiddel As String
        'Public Property byg058SupplerendeVarme As String
        Public Property byg094Revisionsdato As Date
        Public Property byg133KildeTilKoordinatsæt As String
        Public Property byg134KvalitetAfKoordinatsæt As String
        Public Property byg135SupplerendeOplysningOmKoordinatsæt As String
        Public Property byg136PlaceringPåSøterritorie As String
        Public Property byg404Koordinat As String
        Public Property byg406Koordinatsystem As String
        'Public Property forretningshændelse As String
        'Public Property forretningsområde As String
        'Public Property forretningsproces As String
        'Public Property grund As String
        Public Property husnummer As String
        'Public Property id_lokalId As String
        'Public Property id_namespace As String
        'Public Property jordstykke As String
        'Public Property kommunekode As String
        'Public Property registreringFra As Date
        'Public Property registreringsaktør As String
        Public Property status As String
        Public Property virkningFra As Date
        'Public Property virkningsaktør As String
        Public Property etageList() As List(Of Etagelist)
        'Public Property opgangList() As Object
    End Class

    <Serializable> Public Class Etagelist
        Public Property id_lokalId As Object
        Public Property etage As Etage
    End Class

    <Serializable> Public Class Etage
        'Public Property datafordelerOpdateringstid As Date
        'Public Property bygning As String
        Public Property eta006BygningensEtagebetegnelse As String
        Public Property eta025Etagetype As String
        'Public Property forretningshændelse As String
        'Public Property forretningsområde As String
        'Public Property forretningsproces As String
        'Public Property id_lokalId As String
        'Public Property id_namespace As String
        'Public Property kommunekode As String
        'Public Property registreringFra As Date
        'Public Property registreringsaktør As String
        'Public Property status As String
        'Public Property virkningFra As Date
        'Public Property virkningsaktør As String
    End Class

    <Serializable> Public Class Opganglist
        Public Property id_lokalId As String
        Public Property opgang As Opgang
    End Class

    <Serializable> Public Class Opgang
        Public Property datafordelerOpdateringstid As Date
        Public Property adgangFraHusnummer As String
        Public Property bygning As String
        Public Property forretningshændelse As String
        Public Property forretningsområde As String
        Public Property forretningsproces As String
        Public Property id_lokalId As String
        Public Property id_namespace As String
        Public Property kommunekode As String
        Public Property registreringFra As Date
        Public Property registreringsaktør As String
        Public Property status As String
        Public Property virkningFra As Date
        Public Property virkningsaktør As String

    End Class



    Public Enum BygningAnvendelse
        Stuehus_til_landbrugsejendom = 110
        Fritliggende_enfamiliehus = 120
        Sammenbygget_enfamiliehus = 121
        Fritliggende_enfamiliehus_i_tæt = 122
        Række_kæde_eller_dobbelthus = 130
        Række_kæde_og_klyngehus = 131
        Dobbelthus = 132
        Etagebolig = 140
        Kollegium = 150
        Boligbygning_til_døgninstitution = 160
        Anneks_i_tilknytning_til_helårsbolig = 185
        Anden_bygning_til_helårsbeboelse = 190
        Bygning_til_erhvervsmæssig_produktion_vedrørende_landbrug_gartneri_råstofudvinding = 210
        Stald_til_svin = 211
        Stald_til_kvæg_får_mv = 212
        Stald_til_fjerkræ = 213
        Minkhal = 214
        Væksthus = 215
        Lade_til_foder_afgrøder = 216
        Maskinhus_garage = 217
        Lade_til_halm_hø = 218
        Anden_bygning_til_landbrug = 219
        Bygning_til_erhvervsmæssig_produktion_vedrørende_industri_håndværk = 220
        Bygning_til_industri_med_integreret_produktionsapparat = 221
        Bygning_til_industri_uden_integreret_produktionsapparat = 222
        Værksted = 223
        Anden_bygning_til_produktion = 229
        El_gas_vand_eller_varmeværk_forbrændingsanstalt = 230
        Bygning_til_energiproduktion = 231
        Bygning_til_forsyning_og_energidistribution = 232
        Bygning_til_vandforsyning = 233
        Bygning_til_håndtering_af_affald_og_spildevand = 234
        Anden_bygning_til_energiproduktion_og_ = 239
        Anden_bygning_til_landbrug_industri_etc = 290
        Transport_og_garageanlæg = 310
        Bygning_til_jernbane_og_busdrift = 311
        Bygning_til_luftfart = 312
        Bygning_til_parkering_og_transportanlæg = 313
        Bygning_til_parkering_af_flere_end_to_køretøjer_i_tilknytning_til_boliger = 314
        Havneanlæg = 315
        Andet_transportanlæg = 319
        Bygning_til_kontor_handel_lager_herunder_offentlig_administration = 320
        Bygning_til_kontor = 321
        Bygning_til_detailhandel = 322
        Bygning_til_lager = 323
        Butikscenter = 324
        Tankstation = 325
        Anden_bygning_til_kontor_handel_og_lager = 329
        Bygning_til_hotel_restaurant_vaskeri_frisør_og_anden_servicevirksomhed = 330
        Hotel_kro_eller_konferencecenter_med_overnatning = 331
        Bed_and_breakfast = 332
        Restaurant_café_og_konferencecenter_uden_overnatning = 333
        Privat_servicevirksomhed_som_frisør_vaskeri_netcafé = 334
        Anden_bygning_til_serviceerhverv = 339
        Anden_bygning_til_transport_handel_etc = 390
        Bygning_til_biograf_teater_erhvervsmæssig_udstilling_bibliotek_museum_kirke = 410
        Biograf_teater_koncertsted = 411
        Museum = 412
        Bibliotek = 413
        Kirke_eller_anden_bygning_til_trosudøvelse = 414
        Forsamlingshus = 415
        Forlystelsespark = 416
        Anden_bygning_til_kulturelle_formål = 419
        Bygning_til_undervisning_og_forskning = 420
        Grundskole = 421
        Universitet = 422
        Anden_bygning_til_undervisning_og_forskning = 429
        Bygning_til_hospital_sygehjem_fødeklinik = 430
        Hospital_og_sygehus = 431
        Hospice_behandlingshjem = 432
        Sundhedscenter_lægehus_fødeklinik = 433
        Anden_bygning_til_sundhedsformål = 439
        Bygning_til_daginstitution = 440
        Daginstitution = 441
        Servicefunktion_på_døgninstitution = 442
        Kaserne = 443
        Fængsel_arresthus = 444
        Anden_bygning_til_institutionsformål = 449
        Bygning_til_anden_institution_herunder_kaserne_fængsel = 490
        Sommerhus = 510
        Bygning_til_feriekoloni_vandrehjem_bortset_fra_sommerhus = 520
        Feriecenter_center_til_campingplads = 521
        Bygning_med_ferielejligheder_til_erhvervsmæssig_udlejning = 522
        Bygning_med_ferielejligheder_til_eget_brug = 523
        Anden_bygning_til_ferieformål = 529
        Bygning_i_forbindelse_med_idrætsudøvelse = 530
        Klubhus_i_forbindelse_med_fritid_og_idræt = 531
        Svømmehal = 532
        Idrætshal = 533
        Tribune_i_forbindelse_med_stadion = 534
        Bygning_til_træning_og_opstaldning_af_heste = 535
        Anden_bygning_til_idrætformål = 539
        Kolonihavehus = 540
        Anneks_i_tilknytning_til_fritids_og_sommerhus = 585
        Anden_bygning_til_fritidsformål = 590
        Garage_med_plads_til_et_eller_to_køretøjer = 910
        Carport = 920
        Udhus = 930
        Drivhus = 940
        Fritliggende_overdækning = 950
        Fritliggende_udestue = 960
        Tiloversbleven_landbrugsbygning = 970
        Faldefærdig_bygning = 990
        Ukendt_bygning = 999

    End Enum
    Shared Function AnvændelseToOSMBuilding(Anvændelse As BygningAnvendelse) As String
        Select Case Anvændelse
            Case = BygningAnvendelse.Stuehus_til_landbrugsejendom
                Return "farm"
            Case = BygningAnvendelse.Fritliggende_enfamiliehus
                Return "detached"
            Case = BygningAnvendelse.Sammenbygget_enfamiliehus
                Return "terrace"
            Case = BygningAnvendelse.Fritliggende_enfamiliehus_i_tæt
                Return "terrace"
            Case = BygningAnvendelse.Række_kæde_eller_dobbelthus
                Return "terrace"
            Case = BygningAnvendelse.Række_kæde_og_klyngehus
                Return "terrace"
            Case = BygningAnvendelse.Dobbelthus
                Return "semidetached_house"
            Case = BygningAnvendelse.Etagebolig
                Return "apartments"
            Case = BygningAnvendelse.Kollegium
                Return "college"
            Case = BygningAnvendelse.Boligbygning_til_døgninstitution
                Return "apartments"
            Case = BygningAnvendelse.Anneks_i_tilknytning_til_helårsbolig
                Return "shed"
            Case = BygningAnvendelse.Anden_bygning_til_helårsbeboelse
                Return "residential"
            Case = BygningAnvendelse.Bygning_til_erhvervsmæssig_produktion_vedrørende_landbrug_gartneri_råstofudvinding
                Return "farm_auxiliary"
            Case = BygningAnvendelse.Stald_til_svin, BygningAnvendelse.Stald_til_kvæg_får_mv, BygningAnvendelse.Stald_til_fjerkræ, BygningAnvendelse.Minkhal
                Return "barn"
            Case = BygningAnvendelse.Væksthus
                Return "greenhouse"
            Case = BygningAnvendelse.Lade_til_foder_afgrøder
                Return "barn"
            Case = BygningAnvendelse.Maskinhus_garage
                Return "barn"
            Case = BygningAnvendelse.Lade_til_halm_hø
                Return "barn"
            Case = BygningAnvendelse.Anden_bygning_til_landbrug
                Return "farm_auxiliary"
            Case = BygningAnvendelse.Bygning_til_erhvervsmæssig_produktion_vedrørende_industri_håndværk
                Return "industrial"
            Case = BygningAnvendelse.Bygning_til_industri_med_integreret_produktionsapparat, BygningAnvendelse.Bygning_til_industri_uden_integreret_produktionsapparat
                Return "industrial"
            Case = BygningAnvendelse.Værksted
                Return "industrial"
            Case = BygningAnvendelse.Anden_bygning_til_produktion
                Return "industrial"
            Case = BygningAnvendelse.El_gas_vand_eller_varmeværk_forbrændingsanstalt
                Return "industrial"
            Case = BygningAnvendelse.Bygning_til_energiproduktion
                Return "industrial"
            Case = BygningAnvendelse.Bygning_til_forsyning_og_energidistribution
                Return "service"
            Case = BygningAnvendelse.Bygning_til_vandforsyning
                Return "service"
            Case = BygningAnvendelse.Bygning_til_håndtering_af_affald_og_spildevand
                Return "service"
            Case = BygningAnvendelse.Anden_bygning_til_energiproduktion_og_
                Return "industrial"
            Case = BygningAnvendelse.Anden_bygning_til_landbrug_industri_etc
                Return "industrial"
            Case = BygningAnvendelse.Transport_og_garageanlæg
                Return "transportation"
            Case = BygningAnvendelse.Bygning_til_jernbane_og_busdrift
                Return "transportation"
            Case = BygningAnvendelse.Bygning_til_luftfart
                Return "transportation"
            Case = BygningAnvendelse.Bygning_til_parkering_og_transportanlæg
                Return "transportation"
            Case = BygningAnvendelse.Bygning_til_parkering_af_flere_end_to_køretøjer_i_tilknytning_til_boliger
                Return "parking"
            Case = BygningAnvendelse.Havneanlæg
                Return "transportation"
            Case = BygningAnvendelse.Andet_transportanlæg
                Return "transportation"
            Case = BygningAnvendelse.Bygning_til_kontor_handel_lager_herunder_offentlig_administration
                Return "commercial"
            Case = BygningAnvendelse.Bygning_til_kontor
                Return "office"
            Case = BygningAnvendelse.Bygning_til_detailhandel
                Return "retail"
            Case = BygningAnvendelse.Bygning_til_lager
                Return "warehouse"
            Case = BygningAnvendelse.Butikscenter
                Return "supermarket"
            Case = BygningAnvendelse.Tankstation
                Return "commercial"
            Case = BygningAnvendelse.Anden_bygning_til_kontor_handel_og_lager
                Return "commercial"
            Case = BygningAnvendelse.Bygning_til_hotel_restaurant_vaskeri_frisør_og_anden_servicevirksomhed
                Return "commercial"
            Case = BygningAnvendelse.Hotel_kro_eller_konferencecenter_med_overnatning
                Return "hotel"
            Case = BygningAnvendelse.Bed_and_breakfast
                Return "hotel"
            Case = BygningAnvendelse.Restaurant_café_og_konferencecenter_uden_overnatning
                Return "commercial"
            Case = BygningAnvendelse.Privat_servicevirksomhed_som_frisør_vaskeri_netcafé
                Return "commercial"
            Case = BygningAnvendelse.Anden_bygning_til_serviceerhverv
                Return "commercial"
            Case = BygningAnvendelse.Anden_bygning_til_transport_handel_etc
                Return "commercial"
            Case = BygningAnvendelse.Bygning_til_biograf_teater_erhvervsmæssig_udstilling_bibliotek_museum_kirke
                Return "civic"
            Case = BygningAnvendelse.Biograf_teater_koncertsted
                Return "civic"
            Case = BygningAnvendelse.Museum
                Return "civic"
            Case = BygningAnvendelse.Bibliotek
                Return "civic"
            Case = BygningAnvendelse.Kirke_eller_anden_bygning_til_trosudøvelse
                Return "religious"
            Case = BygningAnvendelse.Forsamlingshus
                Return "public"
            Case = BygningAnvendelse.Forlystelsespark
                Return "commercial"
            Case = BygningAnvendelse.Anden_bygning_til_kulturelle_formål
                Return "civic"
            Case = BygningAnvendelse.Bygning_til_undervisning_og_forskning
                Return "yes"
            Case = BygningAnvendelse.Grundskole
                Return "school"
            Case = BygningAnvendelse.Universitet
                Return "university"
            Case = BygningAnvendelse.Anden_bygning_til_undervisning_og_forskning
                Return "yes"
            Case = BygningAnvendelse.Bygning_til_hospital_sygehjem_fødeklinik
                Return "hospital"
            Case = BygningAnvendelse.Hospital_og_sygehus
                Return "hospital"
            Case = BygningAnvendelse.Hospice_behandlingshjem
                Return "hospital"
            Case = BygningAnvendelse.Sundhedscenter_lægehus_fødeklinik
                Return "hospital"
            Case = BygningAnvendelse.Anden_bygning_til_sundhedsformål
                Return "hospital"
            Case = BygningAnvendelse.Bygning_til_daginstitution
                Return "yes"
            Case = BygningAnvendelse.Daginstitution
                Return "kindergarten"
            Case = BygningAnvendelse.Servicefunktion_på_døgninstitution
                Return "yes"
            Case = BygningAnvendelse.Kaserne
                Return "military"
            Case = BygningAnvendelse.Fængsel_arresthus
                Return "government"
            Case = BygningAnvendelse.Anden_bygning_til_institutionsformål
                Return "yes"
            Case = BygningAnvendelse.Bygning_til_anden_institution_herunder_kaserne_fængsel
                Return "yes"
            Case = BygningAnvendelse.Sommerhus
                Return "bungalow"
            Case = BygningAnvendelse.Bygning_til_feriekoloni_vandrehjem_bortset_fra_sommerhus
                Return "yes"
            Case = BygningAnvendelse.Feriecenter_center_til_campingplads
                Return "yes"
            Case = BygningAnvendelse.Bygning_med_ferielejligheder_til_erhvervsmæssig_udlejning
                Return "yes"
            Case = BygningAnvendelse.Bygning_med_ferielejligheder_til_eget_brug
                Return "yes"
            Case = BygningAnvendelse.Anden_bygning_til_ferieformål
                Return "yes"
            Case = BygningAnvendelse.Bygning_i_forbindelse_med_idrætsudøvelse
                Return "sports_hall"
            Case = BygningAnvendelse.Klubhus_i_forbindelse_med_fritid_og_idræt
                Return "yes"
            Case = BygningAnvendelse.Svømmehal
                Return "sports_hall"
            Case = BygningAnvendelse.Idrætshal
                Return "sports_hall"
            Case = BygningAnvendelse.Tribune_i_forbindelse_med_stadion
                Return "yes"
            Case = BygningAnvendelse.Bygning_til_træning_og_opstaldning_af_heste
                Return "riding_hall"
            Case = BygningAnvendelse.Anden_bygning_til_idrætformål
                Return "yes"
            Case = BygningAnvendelse.Kolonihavehus
                Return "bungalow"
            Case = BygningAnvendelse.Anneks_i_tilknytning_til_fritids_og_sommerhus
                Return "shed"
            Case = BygningAnvendelse.Anden_bygning_til_fritidsformål
                Return "yes"
            Case = BygningAnvendelse.Garage_med_plads_til_et_eller_to_køretøjer
                Return "garage"
            Case = BygningAnvendelse.Carport
                Return "carport"
            Case = BygningAnvendelse.Udhus
                Return "shed"
            Case = BygningAnvendelse.Drivhus
                Return "greenhouse"
            Case = BygningAnvendelse.Fritliggende_overdækning
                Return "roof"
            Case = BygningAnvendelse.Fritliggende_udestue
                Return "yes"
            Case = BygningAnvendelse.Tiloversbleven_landbrugsbygning
                Return "yes"
            Case = BygningAnvendelse.Faldefærdig_bygning
                Return "ruins"
            Case = BygningAnvendelse.Ukendt_bygning
                Return "yes"
            Case Else
                Return "yes"
        End Select
    End Function
    Private Shared Function OSMroofMaterial(BBrmateriale As Tagdækningsmateriale) As String
        Select Case BBrmateriale
            Case = Tagdækningsmateriale.TagpapMedLilleHældning, Tagdækningsmateriale.TagpapMedStorHældning
                Return "tar_paper"
            Case = Tagdækningsmateriale.FibercementHerunderAsbest, Tagdækningsmateriale.FibercementUdenAsbest
                Return "eternit"
            Case = Tagdækningsmateriale.Tegl
                Return "roof_tiles"
            Case = Tagdækningsmateriale.Betontagsten
                Return "roof_tiles"
            Case = Tagdækningsmateriale.Metal
                Return "metal_sheet"
            Case = Tagdækningsmateriale.Stråtag
                Return "thatch"
            Case = Tagdækningsmateriale.Glas
                Return "glass"
            Case = Tagdækningsmateriale.LevendeTage
                Return "grass"
            Case = Tagdækningsmateriale.Plastmaterialer
                Return "plastic"
            Case Else
                Return String.Empty
        End Select

    End Function
    Private Shared Function OSMWallMaterial(BBRmateriale As YdervaeggenesMateriale) As String
        Select Case BBRmateriale
            Case = YdervaeggenesMateriale.Mursten
                Return "brick"
            Case = YdervaeggenesMateriale.Letbetonsten
                Return "cement_block"
            Case = YdervaeggenesMateriale.FibercementHerunderAsbest
                Return "concrete"
            Case = YdervaeggenesMateriale.Bindingsværk
                Return "timber_framing"
            Case = YdervaeggenesMateriale.Træ
                Return "wood"
            Case = YdervaeggenesMateriale.Betonelementer
                Return "concrete"
            Case = YdervaeggenesMateriale.Metal
                Return "metal"
            Case = YdervaeggenesMateriale.FibercementUdenAsbest
                Return "concrete"
            Case = YdervaeggenesMateriale.Plastmaterialer
                Return "plastic"
            Case = YdervaeggenesMateriale.Glas
                Return "glass"
            Case = YdervaeggenesMateriale.Ingen
                Return String.Empty
            Case = YdervaeggenesMateriale.Andet
                Return String.Empty
            Case Else
                Return String.Empty
        End Select
    End Function
End Class









