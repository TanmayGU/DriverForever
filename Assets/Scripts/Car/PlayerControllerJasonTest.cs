using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerJasonTest : MonoBehaviour
{
    private CharacterController controller;

    private Vector3 direction;

    public float forwardSpeed = 5f;

    private int desiredLane = 1;
    public float laneDistance = 2.5f;

    public float jumpForce = 10f;
    public float Gravity = -20f;

    private Vector3 originalScale;
    private Vector3 targetScale; // ��̬Ŀ����̬
    public float squeezeSpeed = 5f; // ƽ�������ٶ�

    private AudioSource audioSource; // ��ƵԴ
    private const int sampleRate = 48000;
    private const int sampleSize = 1024;
    private float[] audioSamples = new float[sampleSize];
    public float volumeThreshold = 0.02f; // ������ֵ

    void Start()
    {
        controller = GetComponent<CharacterController>();
        originalScale = transform.localScale;
        targetScale = originalScale;

        // ��� AudioSource ������˷�����
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true; // ʹ��˷�����ѭ������
        audioSource.mute = true; // ����������˷�����ش�������

        if (Microphone.devices.Length > 0)
        {
            string selectedMic = Microphone.devices[0];
            audioSource.clip = Microphone.Start(selectedMic, true, 1, sampleRate);

            if (Microphone.IsRecording(selectedMic))
            {
                Debug.Log($"Microphone '{selectedMic}' is recording.");
                while (!(Microphone.GetPosition(selectedMic) > 0)) { }
                Debug.Log($"Microphone started. Position: {Microphone.GetPosition(selectedMic)}");
                audioSource.Play(); // ȷ�� AudioSource ��ʼ����
            }
            else
            {
                Debug.LogError($"Failed to start recording with Microphone: {selectedMic}");
            }
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    void Update()
    {
        // Forward movement logic
        direction.z = forwardSpeed;

        if (controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            }
        }
        else
        {
            direction.y += Gravity * Time.deltaTime;
        }

        // Lane movement logic
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredLane++;
            if (desiredLane == 3)
                desiredLane = 2;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredLane--;
            if (desiredLane == -1)
                desiredLane = 0;
        }

        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;

        if (desiredLane == 0)
        {
            targetPosition += Vector3.left * laneDistance;
        }
        else if (desiredLane == 2)
        {
            targetPosition += Vector3.right * laneDistance;
        }

        transform.position = targetPosition;

        // ��Ƶ����
        AnalyzeAudio();

        // ƽ�����ɵ�Ŀ�����
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * squeezeSpeed);
    }

    private void FixedUpdate()
    {
        controller.Move(direction * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        direction.y = jumpForce;
    }

    private void AnalyzeAudio()
    {
        if (audioSource.clip == null)
        {
            Debug.Log("AudioSource clip is null. Resetting scale.");
            ResetScale();
            return;
        }

        // ��ȡ��Ƶ����
        audioSource.clip.GetData(audioSamples, 0);
        float volume = GetVolume(audioSamples);

        Debug.Log($"Volume: {volume}");

        // �趨��ֵ
        float silenceThreshold = 0.003f;   // **���������򻷾���������ֵ**
        float lowVolumeThreshold = 0.1f; // **���ڴ�ֵ �� �䰫���**
        float highVolumeThreshold = 0.3f; // **���ڴ�ֵ �� ��߱���**

        if (volume < silenceThreshold) // **������������ԭ״��**
        {
            targetScale = originalScale;
            Debug.Log("Silence detected: Keeping original shape.");
        }
        else if (volume < lowVolumeThreshold) // **�䰫���**
        {
            targetScale = new Vector3(originalScale.x * 1.4f, originalScale.y * 0.6f, originalScale.z);
            Debug.Log("Low Volume detected: Shrinking to wide and short form.");
        }
        else if (volume > highVolumeThreshold) // **��߱���**
        {
            targetScale = new Vector3(originalScale.x * 0.6f, originalScale.y * 1.4f, originalScale.z);
            Debug.Log("High Volume detected: Stretching to tall and thin form.");
        }
        else // **��ͨ����������ԭ״��**
        {
            targetScale = originalScale;
            Debug.Log("Normal Volume: Keeping original shape.");
        }
    }



    private float GetVolume(float[] samples)
    {
        float sum = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }

        float volume = Mathf.Sqrt(sum / samples.Length);
        return volume;
    }

    private void ResetScale()
    {
        targetScale = originalScale; // �ָ�Ĭ�ϱ���
    }
}
