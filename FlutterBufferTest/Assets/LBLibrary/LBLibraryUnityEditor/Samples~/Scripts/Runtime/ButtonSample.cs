using LIBII;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LIBII.Runtime.Samples
{
    public class ButtonSample : MonoBehaviour
    {
        [Button]
        public void Test2(string a, float b)
        {
            Debug.Log("Test2 " + a + " " + b);
        }
        
        [Button]
        public void Test1()
        {
            Debug.Log("Test1");
        }
    }
}