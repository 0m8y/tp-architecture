# Système de Gestion d'Hôtel

Ce projet a pour objectif de développer une application API web en C# pour la gestion d'un hôtel, couvrant divers aspects comme la réservation de chambres, la gestion des clients, le personnel et les services de l'hôtel.

L'objectif est d'appliquer les concepts de programmation orientée objet, les patterns d'architecture et de conception, et de développer une application fonctionnelle et sécurisée.

## Fonctionnalités Attendues

### Client
- **Liste des chambres disponibles** : Obtenir la liste des chambres disponibles à une plage de dates donnée.
- **Réservation de chambre** : Réserver une chambre sur cette plage de date, si elle est disponible, avec possibilité de paiement (numéro de carte bleue, appel à un faux service de paiement).
  - Une chambre a un tarif
  - Une chambre peux accueillir un nombre de personnes
  - Une chambre a un type (simple, double, suite...), qui définit son tarif
  - Un client peut réserver plusieurs chambres selon leur nombre
- **Annulation de réservation** : Annuler sa réservation avec gestion de remboursement sous conditions.
- **Notification pré-séjour** : Optionnel - Recevoir une notification (email/SMS) un jour avant la date du séjour.

### Réceptionniste
- Mêmes fonctionnalités que le client, avec plus d'informations ou de droits:
  - **Liste des chambre disponibles** : Obtenir la liste des chambres disponibles à une plage de dates donnée, avec une information sur l'état général de la chambre (Neuf, Refaite, A refaire, Rien a signaler, Gros dégats).
  - **Annulation de réservation** : Annuler une réservation pour un client, si l'annulation à lieu moins de 48 heures avant la date de réservation, la receptionniste peut choisir de rembourser ou non le client, malgè la règle de base.
- **Gestion de l'arrivée** : Noter l'occupation de la chambre et gérer les paiements non effectués.
- **Gestion du départ** : Marquer la chambre pour nettoyage et gérer les paiements restants.
- **Envoi d'email post-séjour** : Optionel - Envoyer un email type "donnez votre avis" après le départ du client.

### Personnel de Ménage
- **Liste des chambres à nettoyer** : Accéder à la liste des chambres à nettoyer, avec priorisation (une chambre déjà nettoyé et non occupée depuis n'est pas à nettoyer).
- **Marquage des chambres nettoyées** : Noter une chambre comme nettoyée.
- **Notification de casse** : Optionel - Signaler des dommages pour ajustement des frais de paiement.

## Règles Associées
- Les annulations faites moins de 48 heures avant la date de réservation ne seront pas remboursées.
- Les fonctionnalités optionnelles sont encouragées pour les étudiants désirant aller au-delà des exigences de base.
- L'authentification et la gestion des rôles doivent être implémentées pour différencier les acteurs.

## Critères d'Évaluation
- **Architecture et Conception** : Application correcte des patterns d'architecture et de conception discutés en cours.
- **Qualité du Code** : Respect des principes SOLID, respect des principes objets (encapsulation, héritage, composition...).
- **Fonctionnalité** : L'application doit fonctionner comme spécifié, gérer correctement les erreurs et être sécurisée.
- **Documentation** : Un rapport expliquant les choix d'architecture, les défis rencontrés et les solutions adoptées.
- **Optionel** : Tests unitaires, micro services...

## Non évalué
- **Frontend** : L'application doit être une API web, mais un frontend est optionnel.
- **Base de Données** : L'application doit utiliser une base de données (ou un systeme de persistance), mais le choix de la base de données est libre.
- **Authentification** : L'application doit gérer l'authentification, mais le choix de la méthode est libre. Une solution simple de stockage des utilisateurs est suffisante.

## Consignes de Soumission
- Le code source doit être soumis via un dépôt Git ou un zip envoyé par email avant la date limite spécifiée.
- Inclure un fichier `README.md` dans votre dépôt, détaillant comment exécuter l'application et des exemples d'utilisation.
- Soumettre également le rapport de documentation en format PDF.

## Ressources Fournies
- Une base de projet ASP.Net avec une API controller pour servir d'exemple.
- Un projet pour simuler des services de paiement. Le contenu de ce projet ne doit pas/peut pas être modifié.
