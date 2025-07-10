using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class DownloadManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject waitMessage;
    public GameObject downloadMessage;

    public Slider downloadSlider;
    public TextMeshProUGUI sizeInfoText;
    public TextMeshProUGUI downloadValue;

    [Header("Label")]
    public AssetLabelReference defaultLabel;

    private long patchSize;
    private Dictionary<string, long> patchMap = new Dictionary<string, long>();

    private void Start()
    {
        waitMessage.SetActive(true);
        downloadMessage.SetActive(false);
        StartCoroutine(InitAddressable());
        StartCoroutine(CheckUpdateFiles());
    }

    IEnumerator InitAddressable()
    {
        var init = Addressables.InitializeAsync();
        yield return init;
    }

    IEnumerator CheckUpdateFiles()
    {
        var labels = new List<string>() { defaultLabel.labelString };
        patchSize = default;
        foreach (var label in labels)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);
            yield return handle;
            patchSize += handle.Result;
        }
        if(patchSize > decimal.Zero)
        {
            waitMessage.SetActive(false);
            downloadMessage.SetActive(true);
            sizeInfoText.text = GetFileSize(patchSize);
        }
        else
        {
            downloadValue.text = " 100 % ";
            downloadSlider.value = 1f;
            yield return new WaitForSeconds(2f);
            LoadingManager.LoadScene("SampleScene");
        }
    }

    private string GetFileSize(long byteCnt)
    {
        string size = "0 Bytes";
        if (byteCnt >= 1073741824.0)
        {
            size = string.Format("{0:##.##}", byteCnt / 1073741824.0) + " GB";
        }
        else if (byteCnt >= 1048576.0)
        {
            size = string.Format("{0:##.##}", byteCnt / 1048576.0) + " MB";
        }
        else if(byteCnt >= 1024.0)
        {
            size = string.Format("{0:##.##}", byteCnt / 1024.0) + " KB";
        }
        else if(byteCnt > 0 && byteCnt < 1024.0)
        {
            size = byteCnt.ToString() + " Bytes";
        }
        return size;
    }

    public void Button_DownLoad()
    {
        StartCoroutine(PatchFiles());
    }

    IEnumerator PatchFiles()
    {
        var labels = new List<string>() { defaultLabel.labelString };
        foreach (var label in labels)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);
            yield return handle;
            if (handle.Result != decimal.Zero) StartCoroutine(DownloadLabel(label));
        }
        yield return CheckDownload();
    }

    IEnumerator DownloadLabel(string label)
    {
        patchMap.Add(label, 0);
        var handle = Addressables.DownloadDependenciesAsync(label, false);
        while (!handle.IsDone)
        {
            patchMap[label] = handle.GetDownloadStatus().DownloadedBytes;
            yield return new WaitForEndOfFrame();
        }
        patchMap[label] = handle.GetDownloadStatus().TotalBytes;
        Addressables.Release(handle);
    }

    IEnumerator CheckDownload()
    {
        var total = 0f;
        downloadValue.text = "0 %";
        while (true)
        {
            total += patchMap.Sum(tmp => tmp.Value);
            downloadSlider.value = total / patchSize;
            downloadValue.text = (int)(downloadSlider.value * 100) + " %";
            if(total == patchSize)
            {
                LoadingManager.LoadScene("SampleScene");
                break;
            }
            total = 0f;
            yield return new WaitForEndOfFrame();
        }
    }
}
