using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace PassGuard.Api.Service;

public class AESService
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes(AESKey.LoadAESKey());
    
    public static string EncryptString(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.GenerateIV();
        var iv = aes.IV;

        // Création d'un encryptor
        using var encryptor = aes.CreateEncryptor(aes.Key, iv);

        using var ms = new MemoryStream();
        // On écrit d'abord l'IV dans le MemoryStream (pour pouvoir le récupérer lors du déchiffrement)
        ms.Write(iv, 0, iv.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        // Le résultat est le contenu du MemoryStream (IV + données chiffrées), encodé en Base64
        var result = Convert.ToBase64String(ms.ToArray());
        return result;
    }

    public static string DecryptString(string cipherText)
    {
        // Convertir la chaîne chiffrée depuis Base64
        var fullCipher = Convert.FromBase64String(cipherText);

        using Aes aes = Aes.Create();
        aes.Key = Key;
        // Extraire le vecteur d'initialisation (IV)
        byte[] iv = new byte[aes.BlockSize / 8];
        Array.Copy(fullCipher, 0, iv, 0, iv.Length);
        aes.IV = iv;

        // Les données chiffrées commencent après l'IV
        int cipherStartIndex = iv.Length;
        int cipherLength = fullCipher.Length - cipherStartIndex;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(fullCipher, cipherStartIndex, cipherLength);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);

        return sr.ReadToEnd();
    }
}