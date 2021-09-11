# SDFE2OSM
Imports information from the Danish government into OpenStreetMap
Text is in Danish because the software only applies to Denmark

# Hvad laver programmet:
+ kopiere data fra BBR til OSM på en semi-automatisk måde.  du skal selv validere og uploade
+ tilføjer FIXME tag til bygninger som er revet ned ifølge BBR (desværre passer det ikke altid med sandheden)
+ for sammenlimede bygninger tilføjer den kun de ting alle bygninger har tilfælles. Hold da kæft der findes mange carporte i Danmark.
+ respekterer eksisterende tags, med undtagelse af "building:*"  Denne tag vil blive upgradet (yes -> appartments) men aldrig downgraded (appartments -> yes).
+ mulighed for macros "alle bygninger med tag i tagpap har gråt tag"
+ lige nu kan den ikke tegne bygninger, kun tilføje tags
+ se resultatet i 3D viewere som for eksempel https://osmbuildings.org/

# Kræver:
+ Windows med admin rettigheder
+ JOSM
+ Proj  (open source)

Start med at åbne en konto på www.datafordeler.dk
Du SKAL logge ind med Nem-ID
Så opretter du en "web-bruger" IKKE certificat men user/pwd
De vil give dig en brugernavn, du vælger selv password
Dem skal du taste ind i programmet, og så [Test]

Du skal installere PROJ via OSgeo4W.
Tjek denne fil findes: "C:\OSGeo4W\bin\cs2cs.exe"

# Hvordan virker det?

I JOSM, selecter 1 til 10 byginger og deres adresse noder
tip: <Ctrl>F   og så filter på:    building=* or osak\:identifier=*
Når du har de rigtige selection, tryk <Ctrl>C for at kopiere på klembord.
Programmet vil se at der er noget på klembordet, og gå i gang.
Senere kan du tage flere byginger ad gangen, når du har fået fornemmelse om hvor lang tid det tager.

Programmet cacher lidt data, hvad hjælper hvis du kører mere end 1 gange.  
