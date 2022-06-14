# BitrixSDK
Create Client:
```cs
public IBitrixService BitrixCreateClient()
{
      var configCredential = configuration.GetSection("Credential");
      var credential = new System.Net.NetworkCredential(configCredential["login"], configCredential["password"]);
      return new BitrixClient(
          new AuthParameters(
              new Site24(configuration["Url"]),
              credential,
              configuration["ClientId"],
              configuration["ClientSecret"])
          ) as IBitrixService;
}
```
