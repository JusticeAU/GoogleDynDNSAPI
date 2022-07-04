# GoogleDynDNSAPI
Updates your google dynamic DNS IP every 5 minutes

## How to use
On run, if a configuration file is not detected, you will be prompted for credentials.

If you don't know these, you can create and retrieve them from the 'View Credentials' option in your Dynamic DNS configuration.

On entering the username, password and hostname (you dynamic dns name) a configuration file will be generated.
The username and password will be Base64 encoded.

The tool will then immediately look up you IP address, and make an API call to Google to update your dynamic DNS to this IP address.

It will store your last known IP address.

The next time the application runs, it will check if the IP has changed, and if so, load the configuration file and make an API call.

It will not make a call if your IP has not changed (or lastip.cfg is missing)

You can create a scheduled task to run this application every 5 minutes on boot.
