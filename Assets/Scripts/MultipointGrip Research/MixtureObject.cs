using System.Collections;
using UnityEngine;

public class MixtureObject : MonoBehaviour
{
    enum GripFluid
    {
        RED,
        BLUE,
        YELLOW
    }

    public MonotypeUnityTextPlugin.MP3DTextComponent[] mixture_amts;    //for showing Unity inspector
    public MonotypeUnityTextPlugin.MP3DTextComponent[] mixture_totals;  //for showing Unity inspector
    [HideInInspector]public float[] mixtureData;

    [Space(10)]
    public int maxAmount;

    private GripFluid m_fluidState;
    private int mixtureNum;
    private float fluidHeight;
    private float maxFluidHeight;
    private float particleCollisionCnt = 0;

    void Start()
    {
        SetMixtureRatio();
    }

    void FixedUpdate()
    {
        if(particleCollisionCnt >= 3)
        {
            if (fluidHeight > maxFluidHeight)
                return;

            UpdateMixtureBoard();
            UpdateMixture();
            particleCollisionCnt = 0;

            //FinishButtonManager.canBeClicked = true;
        }
    }

    private void SetMixtureRatio()
    {
        mixtureNum = mixture_amts.Length;
        fluidHeight = transform.localScale.z;
        switch (maxAmount)
        {
            case 210:
                maxFluidHeight = 5.2f;
                mixture_totals[0].Text = 80.ToString() + "ml";
                mixture_totals[1].Text = 40.ToString() + "ml";
                mixture_totals[2].Text = 90.ToString() + "ml";
                break;
            case 90:
                maxFluidHeight = 3.2f;
                mixture_totals[0].Text = 30.ToString() + "ml";
                mixture_totals[1].Text = 25.ToString() + "ml";
                mixture_totals[2].Text = 35.ToString() + "ml";
                break;
            case 50:
                maxFluidHeight = 2.5f;
                mixture_totals[0].Text = maxAmount.ToString() + "ml";
                break;
            default: 
                Debug.Log("Should define maxAmount of beaker");
                maxFluidHeight = 1f;
                mixture_totals[0].Text = 0.ToString();
                mixture_totals[1].Text = 0.ToString();
                mixture_totals[2].Text = 0.ToString();
                break;
        }

    }

    private void UpdateMixtureBoard()
    {
        float curAmt;

        if (mixture_amts.Length == 1)
        {
            curAmt = float.Parse(mixture_amts[0].Text);
            mixture_amts[0].Text = (++curAmt).ToString();
        }
        else
        {
            switch (m_fluidState)
            {
                case GripFluid.RED:
                    curAmt = float.Parse(mixture_amts[0].Text);
                    mixture_amts[0].Text = (++curAmt).ToString();
                    break;
                case GripFluid.YELLOW:
                    curAmt = float.Parse(mixture_amts[1].Text);
                    mixture_amts[1].Text = (++curAmt).ToString();
                    break;
                case GripFluid.BLUE:
                    curAmt = float.Parse(mixture_amts[2].Text);
                    mixture_amts[2].Text = (++curAmt).ToString();
                    break;
            }
        }
    }

    private void UpdateMixture()
    {
        fluidHeight += 0.02f;
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, fluidHeight);
    }

    void OnParticleCollision(GameObject other)
    {
        particleCollisionCnt++;

        if (other.name.Equals("FlowFluid_B"))
            m_fluidState = GripFluid.BLUE;
        else if (other.name.Equals("FlowFluid_Y"))
            m_fluidState = GripFluid.YELLOW;
        else if (other.name.Equals("FlowFluid_R"))
            m_fluidState = GripFluid.RED;
    }

    public void SetMixtureData()
    {
        mixtureData = new float[mixtureNum * 2];

        for(int i = 0; i< mixtureData.Length / 2; i++)
            mixtureData[i] = float.Parse(mixture_amts[i].Text);

        int j = 0;
        for (int i = mixtureData.Length / 2; i < mixtureData.Length; i++)
        {
            mixtureData[i] = float.Parse(mixture_totals[j].Text.Split('m')[0]);
            j++;
        }
    }
}
