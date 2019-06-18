using UnityEngine;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

public class TextRijndael : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        byte[] bytes = new byte[4] { 100,99,98,97 };
        RijndaelManaged rijma = new RijndaelManaged();
        rijma.GenerateKey();
        rijma.GenerateIV();
        Rijndael rij = new Rijndael();
        byte[] enbytes = rij.Encrypt(bytes, rijma.Key, rijma.IV);
       
   
        byte[] debytes = rij.Decrypt(enbytes, rijma.Key, rijma.IV);
      
    }

    // Update is called once per frame
    void Update()
    {

    }
}
