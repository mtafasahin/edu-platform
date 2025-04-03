export const environment = {
    production: false,
    // keycloakUrl: 'http://localhost:8080',
    // keycloakRealm: 'edu-platform',
    // keycloakClientId: 'kong-client',
    keycloak: {
        enable: true, //Enable or disable Keycloak for Frontend app
        authority: 'http://localhost:8080', //Keycloak URL
        redirectUri: 'http://localhost:4200', //Frontend app URL
        postLogoutRedirectUri: 'http://localhost:4200/logout', //Optional value
        realm: 'edu-platform', //Realm name
        clientId: 'kong-client',
      }
  };
  