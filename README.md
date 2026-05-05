# wi_game_techpriests

Projekt gry promującej Wydział Informatki AGH tworzony przez zespół *TechKapłani* w ramach przedmiotu *Inżynieria Oprogramowania*.

## Definition of done:

#### Rozgrywka

* Rozgrywka każdej z minigier działa z osobna
* Gracz dołącza do rozgrywki po podaniu kodu sesji i unikalnego nicku
* Można przechodzić pomiędzy kolejnymi etapami rozgrywki (w jednym kierunku))
* Jest możliwe zagranie ponownie w daną minigrę (max 3 razy)
* Po każdej rozgrywce w minigrę otrzymujemy informację o wyniku
* Pod koniec rozgrywki otrzymujemy informację o całkowitym wyniku
* Możemy zobaczyć nasz wynik w porównaniu z innymi graczami (z podziałem na poszczegolne minigry + całkowity) na leaderboardzie

#### Backend

* Można tworzyć wiele sesji w tym samym czasie.
* Gracz nie może się zapisać do nieaktywnej/nieistniejącej sesji
* Gracz nie może mieć nicku już istniejącego w ramach danej sesji
* Tylko zapisani gracze mają dostęp do pobierania danych minigier i publikowania wyniku (token)
* Gracz może podejrzeć wyniki graczy wyłącznie z sesji do której jest zapisany
* Dane do minigier są wybierane losowo z bazy danych
