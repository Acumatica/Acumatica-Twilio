[![Project Status](http://opensource.box.com/badges/active.svg)](http://opensource.box.com/badges)

Acumatica Twilio Integration
==================================

Extension that allows to send SMS and Out-bound call type of notification from Acumatica utilizing Twilio API. With this extension, you can do the following:
* Send SMS to billing contact of AR Invoice which has balance due.
* Send Out-bound call to billing contact of AR Invoice which has balance due.

### Prerequisites
* You must have Twilio account 

Quick Start
-----------

### Installation

##### Step 1: Setup your Twilio account
1. Create Twilio Account - https://www.twilio.com
2. Once Account is created, you can find API Credentials at https://www.twilio.com/console/account/settings

##### Step 2: Install the customization project
1. Download PXTwilioIntegrationPkg.zip from this repository
2. In your Acumatica ERP instance, import PXTwilioIntegrationPkg.zip as a customization project
3. Publish the customization project

##### Step 3: Create a notification template/s in Acumatica for the SMS and Out-bound Call that will be sent via Twilio
1. Go to the Notification Templates screen (Configuration -> Email -> Notification Templates)(SM204003)
2. Create the template. For example, it might look like this:
![Notification Template Example](/_ReadMeImages/SM204003.png)

### Configuration
1. Go to System -> Management -> Configure -> Twilio Integration (TW101000)
2. Specify Account SID, Auth Token and From Phone # from your Twilio Account.
3. Specify the Notification Template to the one you created for SMS and Out-bound call.
4. Save the changes to the page.
![Twilio Integration](/_ReadMeImages/TW101000.png)

#### Usage

To use this feature:

1. Go to Finance -> Accounts Receivable -> Enter -> Invoices & Memos (AR301000) and Create AR Invoice.
2. Release AR Invoice. Once Released, below options will be enabled.
   * Actions -> Send SMS Notification
   * Actions -> Send Call Notification
![Advanced Notification Options](/_ReadMeImages/AR301000.png)
3. Notifications can be scheduled for released AR Invoices having balance due from Finance -> Accounts Receivable -> Processes -> Daily -> Send SMS/Call Notification (TW501000)
![Advanced Notification Options](/_ReadMeImages/TW501000.png)
4. Once a notification has been sent, a new activity is create as well. 
![Advanced Notification Options](/_ReadMeImages/AR301000Activities.png)

Known Issues
------------
None at the moment

## Copyright and License

Copyright © `2017` `Acumatica`

This component is licensed under the MIT License, a copy of which is available online [here](LICENSE.md)
