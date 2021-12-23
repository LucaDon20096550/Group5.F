# WDPR Project

### Processen

#### PDP (10%)

+ Softwareontwikkelprocess
  - Scrum
  - Brainstorm
  - Teamafspraken nakomen
  - Reflectie
  - Samenwerking

#### Webdevelopment (60%)

+ Omvang van het product
+ Correcte toepassing van bootstrap (usability)
+ Een goed website (accessibility)
+ Telefoon en op laptop responsive
+ Correcte toepassing van
  - Jquery
  - Ajax
  - Json
  - ObjectResult
+ Kwaliteit van c# code
  - Onderhoudbaarheid
  - Schaalbaarheid
  - Commentaar
+ Correcte realisatie (o.b.v Code first en EF), ( One2Many en Many2Many relation-- minimaal 1 van beiden )
+ Realisatie van Crud operaties voor Admin o.b.v. scaffolden
+ Security by Design
  - Https
  - Database
  - XSS aanvallen
  - Front-end Security
  - Identity
+ Realisatie van Authenticatie m.b.v. Identity Provider
+ Zoekfunctie
  - is effectief
  - is efficient
  - is schaalbaar
+ Correcte toepassing van MVC
+ Gebruik van Layouts en Partial Views
+ Tussenproducten worden gepublished naar Azure
+ Gebruik van GIT
+ Gebruik van Xunit en Moq voor Unittesten
+ Testplan
  - Integratie test
  - Performance test

#### Database (30%)

+ Domein model in uml
+ Database normalisatie
+ Het gebruik van sql
  - SQL queries
  - Triggers
  - Views
  - Checks

### Code Conventies

#### Pascal Case
Bij het aanmaken van een Class, Var en etc.

Onjuist:

````
public class pascalcase 
{
  public string firstname;
  public string Stateorprovince;
}
````

Juist:

````
public class PascalCase 
{
  public string Firstname;
  public string StateOrProvince;
}
````

#### Spacing
Maak gebruik van 1 tab achter lines wanneer je in een scope zit en 1 white line na methoden.

Onjuist:

````
public class PascalCase 
{
public string Firstname;
public string StateOrProvince;
}
````

Juist:

````
public class PascalCase 
{
  public string Firstname;
  public string StateOrProvince;
}
// white line
````

#### Comments
Het gebruiken van comments.

Onjuist:

````
//this is a
//comment.
````

Juist:

````
// This is a
// comment.
````

#### Taal
De taal tijdens het coderen word gedaan in het engels.

Onjuist:

````
public void StudentToevoegen()
{
  
}

string Voornaam;
````

Juist:

````
public void AddStudent()
{
  
}

string Firstname;
````

#### Methoden
Het openen en closen van methoden.

Onjuist:

````
public void AddStudent() {
  
}
````

Juist:

````
public void AddStudent()
{
  
}
````

### Handleiding

{ Einde van product vermeld }
