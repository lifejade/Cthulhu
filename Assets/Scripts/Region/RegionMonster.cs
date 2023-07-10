using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RegionMonster : MonoBehaviour
{
    public RegionNode here {
        get { return _here; }
        set { _here = value;
            this.transform.position = new Vector3(_here.transform.position.x, _here.transform.position.y, -3); 
        }
    }
    private RegionNode _here = null;
    private SpriteRenderer render;

    public bool isActive = false;
    public RegionNode origin;


    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        render.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void chasePlayer()
    {
        if (!isActive)
            return;

        RegionNode player = Managers.Region.PlayerIsIn;
        RegionNode nextmovenode = player;

        Dictionary<RegionNode, int> solution = new Dictionary<RegionNode, int>();
        Queue<RegionNode> q = new Queue<RegionNode>();
        Dictionary<RegionNode, RegionNode> pre = new Dictionary<RegionNode, RegionNode>();

        solution.Add(here, 0);
        q.Enqueue(here);
        pre.Add(here, here);

        while (q.Count != 0)
        {
            RegionNode q_node = q.Dequeue();
            int dist = solution[q_node];
            foreach (GraphEdgeNode n in q_node.adj_list)
            {
                RegionNode node = (RegionNode)n.opposite(q_node);
                if (node.board_State == RegionNode.Board_State.ban)
                    continue;
                if (node.board_State == RegionNode.Board_State.monsteronlyban && player != node)
                    continue;


                if (solution.ContainsKey(node))
                    continue;

                q.Enqueue(node);
                solution.Add(node, dist + 1);
                pre.Add(node, q_node);
            }
        }

        while (pre[nextmovenode] != here)
        {
            nextmovenode = pre[nextmovenode];
        }
        here = nextmovenode;

        if (solution[here] <= Managers.Region.sight)
        {
            render.enabled = true;
        }
        else
        {
            render.enabled = false;
        }
        Managers.Region.MonsterInNamesLast[origin.gameObject.name]= here.gameObject.name;
    }

}
