# Event-Driven-BankAccount-Opening-Application

SETTLEMENT WORKFLOW 
USE CASE
 This details the problem statement this solution intends to solve. 
* As a user on the platform I want to begin the process to create an account by clicking on the create account button.
  This action will make a call to the API that will then post a message on the PotentialMember Queue to be picked up by the PotentialMember WebJob/Azure Function and the data will be stored. This does not affect the application flow, it is stored for business insight purposes.

* As a user, I want to continue with the account creation process if I am comfortable with the terms and conditions by clicking on a Get started button. No server call is made at this point.

* As a user I should be able to enter relevant information on the sign up for the account opening process. No server call is made at this point

* As a user I should be able to click on the submit button and I should get a response that my request is undergoing processing or an error message in case of a failure.
  This action will make a call to the API that will then post a message on the CreateAccount Queue to be picked up by the CreateAccount WebJob and the following processes will occur.
  Verify email unicity in the account table in the database
  Get customer credit score from the db if it exists
  Create an account for the user with the given details and credit score
  Post a message to the CommunicateWithMember Queue

* As a user I should receive the details of my account via email
  The user should get an email on the email he/she used to create the account with the details of the recently opened account or errors if any.
 
 # Technologies Used
 
 * C#
 * .NET 6
 * Azure ServiceBus
 * Azure Webjobs
 * Azure Functions
 * MSSQL
 * React (For minimalistic UI)
 
 
# Implementation Diagram
![Screenshot_20221126_022521](https://user-images.githubusercontent.com/72900885/207101727-ee116655-6f77-4066-a5af-0a25f0e407ce.png)
