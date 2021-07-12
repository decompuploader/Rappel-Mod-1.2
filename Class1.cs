using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace climbrope
{
  public class Class1 : Script
  {
    private bool activated = false;
    private Vector3 aimpos;
    private Rope rop = (Rope) null;
    private Prop prop = (Prop) null;
    private float distance;
    private float rplength;
    private bool ropeOn = false;
    private Vector3 lefthand;
    private float headto;

    public Class1()
    {
      this.Tick += new EventHandler(this.OnTick);
      this.KeyDown += new KeyEventHandler(this.OnKeyDown);
      this.KeyUp += new KeyEventHandler(this.OnKeyUp);
      this.Interval = 5;
    }

    private void RotToPos(Vector3 postoface)
    {
      Vector3 vector3 = postoface;
      Vector3 position = Game.Player.Character.Position;
      Game.Player.Character.Heading = Function.Call<float>(Hash._0x2FFB6B224F4B2926, (InputArgument) (vector3.X - position.X), (InputArgument) (vector3.Y - position.Y));
    }

    public void OnTick(object sender, EventArgs e)
    {
      this.OnKeyDown();
      this.OnKeyUp();
      if (!this.activated || !this.ropeOn)
        return;
      Game.DisableControlThisFrame(2, GTA.Control.Cover);
      Game.DisableControlThisFrame(2, GTA.Control.Jump);
      Game.DisableControlThisFrame(2, GTA.Control.Sprint);
      Game.DisableControlThisFrame(2, GTA.Control.Attack);
    }

    public void OnKeyDown()
    {
      Ped character = Game.Player.Character;
      if (!character.IsOnFoot)
        return;
      if (Game.IsControlPressed(2, GTA.Control.Sprint) && Game.IsControlJustPressed(2, GTA.Control.Context))
      {
        if (this.activated)
        {
          this.activated = false;
          if (this.ropeOn)
          {
            this.rop.Delete();
            this.prop.Delete();
            this.ropeOn = false;
            character.CanRagdoll = true;
            Function.Call(Hash._0x9FF447B6B6AD960A, (InputArgument) character, (InputArgument) true);
            character.Task.ClearAll();
          }
          UI.ShowHelpMessage("Rappel Mod: Disabled");
        }
        else
        {
          this.activated = true;
          UI.ShowHelpMessage("Rappel Mod: Enabled");
        }
      }
      if (this.activated)
      {
        Game.DisableControlThisFrame(2, GTA.Control.Aim);
        if (Game.IsControlPressed(2, GTA.Control.Aim))
        {
          RaycastResult raycastResult1 = World.Raycast(character.Position + character.ForwardVector * 1f, character.UpVector * -1f, 3f, IntersectOptions.Everything, (Entity) Game.Player.Character);
          if (raycastResult1.DitHitAnything)
          {
            this.aimpos = raycastResult1.HitCoords;
            World.DrawMarker(MarkerType.DebugSphere, raycastResult1.HitCoords, Vector3.Zero, Vector3.Zero, new Vector3(0.4f, 0.4f, 0.4f), Color.Blue);
          }
          if (!raycastResult1.DitHitAnything && !this.ropeOn)
          {
            character.Task.ClearAllImmediately();
            Function.Call<Vector3>(Hash._0x06843DA7060A026B, (InputArgument) character, (InputArgument) character.Position.X, (InputArgument) character.Position.Y, (InputArgument) ((double) character.Position.Z - 0.7), (InputArgument) 1, (InputArgument) 1, (InputArgument) 1, (InputArgument) 0);
            Function.Call(Hash._0x9FF447B6B6AD960A, (InputArgument) character, (InputArgument) false);
            character.CanRagdoll = false;
            Script.Wait(10);
            character.Task.PlayAnimation("mp_common_heist", "rappel_intro", 8f, 1f, -1, AnimationFlags.StayInEndFrame, 0.0f);
            Script.Wait(3600);
            Function.Call<Vector3>(Hash._0x06843DA7060A026B, (InputArgument) character, (InputArgument) character.Position.X, (InputArgument) character.Position.Y, (InputArgument) (character.Position.Z - 1.35f), (InputArgument) 1, (InputArgument) 1, (InputArgument) 1, (InputArgument) 0);
            Script.Wait(100);
            Game.Player.Character.Task.PlayAnimation("missrappel", "rappel_idle", 4f, 1f, -1, AnimationFlags.StayInEndFrame, 0.0f);
            character.ApplyForce(character.ForwardVector * 6f);
            Script.Wait(200);
            RaycastResult raycastResult2 = World.RaycastCapsule(character.Position + new Vector3(0.0f, 0.0f, -2.7f), character.Position + new Vector3(0.0f, 0.0f, -2.7f), 1f, IntersectOptions.Everything, (Entity) character);
            if (raycastResult2.DitHitAnything)
            {
              this.RotToPos(raycastResult2.HitCoords);
              Script.Wait(50);
              this.headto = character.Heading;
            }
            this.prop = World.CreateProp((Model) "prop_ashtray_01", new Vector3(character.Position.X + character.ForwardVector.X * 0.17f, character.Position.Y + character.ForwardVector.Y * 0.17f, character.Position.Z + 0.5f), true, false);
            this.prop.FreezePosition = true;
            this.prop.IsVisible = false;
            this.distance = 1000f;
            this.rop = World.AddRope((RopeType) 0, character.Position, character.UpVector, this.distance, 0.25f, false);
            Script.Wait(30);
            this.rop.ActivatePhysics();
            this.lefthand = new Vector3(character.GetBoneCoord(Bone.IK_L_Hand).X + character.ForwardVector.X * -0.07f, character.GetBoneCoord(Bone.IK_L_Hand).Y + character.ForwardVector.Y * -0.07f, character.GetBoneCoord(Bone.IK_L_Hand).Z);
            this.rop.AttachEntities((Entity) character, this.lefthand, (Entity) this.prop, this.prop.Position, this.distance);
            this.ropeOn = true;
          }
        }
        if (this.ropeOn && Game.IsControlJustPressed(2, GTA.Control.Enter))
        {
          character.Task.ClearAll();
          character.Task.PlayAnimation("missheistfbi3b_ig9", "rappel_dismount_franklin", 5f, 2f, 1000, AnimationFlags.AllowRotation, 0.0f);
          this.rop.Delete();
          this.prop.Delete();
          this.ropeOn = false;
          character.CanRagdoll = true;
          Function.Call(Hash._0x9FF447B6B6AD960A, (InputArgument) character, (InputArgument) true);
        }
        int num1;
        if (!Function.Call<bool>(Hash._0x1F0B79228E461EC9, (InputArgument) character, (InputArgument) "missrappel", (InputArgument) "rappel_walk", (InputArgument) 1))
          num1 = Function.Call<bool>(Hash._0x1F0B79228E461EC9, (InputArgument) character, (InputArgument) "missrappel", (InputArgument) "rappel_idle", (InputArgument) 1) ? 1 : 0;
        else
          num1 = 1;
        if (num1 != 0 && ((double) character.Position.Z > (double) this.prop.Position.Z - 2.5 && Game.IsControlPressed(2, GTA.Control.MoveUpOnly) && Game.IsControlJustPressed(2, GTA.Control.Jump)))
        {
          character.Task.ClearAllImmediately();
          character.Task.Climb();
          this.rop.Delete();
          this.prop.Delete();
          character.CanRagdoll = true;
          this.ropeOn = false;
          Function.Call(Hash._0x9FF447B6B6AD960A, (InputArgument) character, (InputArgument) true);
        }
        int num2;
        if (this.ropeOn)
        {
          if (Function.Call<bool>(Hash._0x1F0B79228E461EC9, (InputArgument) character, (InputArgument) "missrappel", (InputArgument) "rappel_walk", (InputArgument) 1) && Game.IsControlPressed(2, GTA.Control.MoveUpOnly))
          {
            num2 = (double) character.Position.Z > (double) this.prop.Position.Z - 1.0 ? 1 : 0;
            goto label_28;
          }
        }
        num2 = 0;
label_28:
        if (num2 != 0)
        {
          Game.Player.Character.Task.PlayAnimation("missrappel", "rappel_idle", 4f, 1f, -1, AnimationFlags.StayInEndFrame, 0.0f);
          Function.Call(Hash._0xCB2D4AB84A19AA7C, (InputArgument) this.rop);
          Function.Call(Hash._0xFFF3A50779EFBBB3, (InputArgument) this.rop);
        }
        if (this.ropeOn && Game.IsControlPressed(2, GTA.Control.MoveUpOnly) && !Game.IsControlPressed(2, GTA.Control.Attack) && (double) character.Position.Z < (double) this.prop.Position.Z - 1.0)
        {
          if (Game.IsControlJustPressed(2, GTA.Control.MoveUpOnly))
          {
            character.Task.ClearSecondary();
            character.Task.PlayAnimation("missrappel", "rappel_walk", 7f, 1f, -1, AnimationFlags.Loop, 0.0f);
            Function.Call(Hash._0x1461C72C889E343E, (InputArgument) this.rop);
          }
          this.rplength = this.rop.Length - 0.05f;
          Function.Call(Hash._0xD009F759A723DB1B, (InputArgument) this.rop, (InputArgument) this.rplength);
          Function.Call<Vector3>(Hash._0x06843DA7060A026B, (InputArgument) character, (InputArgument) character.Position.X, (InputArgument) character.Position.Y, (InputArgument) (float) ((double) character.Position.Z - 1.0 + 0.0549999997019768), (InputArgument) 1, (InputArgument) 1, (InputArgument) 1, (InputArgument) 0);
        }
        if (this.ropeOn && Game.IsControlPressed(2, GTA.Control.MoveDownOnly) && !Game.IsControlPressed(2, GTA.Control.Attack))
        {
          if (Game.IsControlJustPressed(2, GTA.Control.MoveDownOnly))
          {
            character.Task.ClearSecondary();
            character.Task.PlayAnimation("missrappel", "rappel_walk", 7f, 1f, -1, AnimationFlags.Loop, 0.0f);
            Function.Call(Hash._0x538D1179EC1AA9A9, (InputArgument) this.rop);
          }
          this.rplength = this.rop.Length + 0.15f;
          Function.Call(Hash._0xD009F759A723DB1B, (InputArgument) this.rop, (InputArgument) this.rplength);
          character.ApplyForce(new Vector3(0.0f, 0.0f, -0.75f));
        }
        if (this.ropeOn && Game.IsControlPressed(2, GTA.Control.Attack))
        {
          if (Game.IsControlJustPressed(2, GTA.Control.Attack))
          {
            character.Task.ClearSecondary();
            character.Task.PlayAnimation("missrappel", "rope_slide", 5f, 1f, -1, AnimationFlags.Loop, 0.0f);
            this.rop.DetachEntity((Entity) character);
            this.lefthand = new Vector3(character.GetBoneCoord(Bone.IK_L_Hand).X + character.ForwardVector.X * -0.26f, character.GetBoneCoord(Bone.IK_L_Hand).Y + character.ForwardVector.Y * -0.26f, character.GetBoneCoord(Bone.IK_L_Hand).Z);
            this.rop.AttachEntities((Entity) character, this.lefthand, (Entity) this.prop, this.prop.Position, this.distance);
            Function.Call(Hash._0x538D1179EC1AA9A9, (InputArgument) this.rop);
          }
          this.rplength = this.rop.Length + 1f;
          Function.Call(Hash._0xD009F759A723DB1B, (InputArgument) this.rop, (InputArgument) this.rplength);
          character.ApplyForce(new Vector3(0.0f, 0.0f, -0.9f));
        }
        int num3;
        if (this.ropeOn && !Game.IsControlPressed(2, GTA.Control.MoveUpOnly))
        {
          if (!Function.Call<bool>(Hash._0x1F0B79228E461EC9, (InputArgument) character, (InputArgument) "missrappel", (InputArgument) "rope_slide", (InputArgument) 1))
          {
            num3 = Game.IsControlJustPressed(2, GTA.Control.Jump) ? 1 : 0;
            goto label_46;
          }
        }
        num3 = 0;
label_46:
        if (num3 != 0)
        {
          Function.Call(Hash._0x538D1179EC1AA9A9, (InputArgument) this.rop);
          this.rplength = this.rop.Length + 3f;
          Function.Call(Hash._0xD009F759A723DB1B, (InputArgument) this.rop, (InputArgument) this.rplength);
          character.ApplyForce(character.ForwardVector * -10f);
          Script.Wait(250);
          character.ApplyForce(new Vector3(0.0f, 0.0f, -15f));
          Script.Wait(300);
          character.Task.PlayAnimation("missrappel", "rappel_jump_c", 4f, 1f, 130, AnimationFlags.AllowRotation, 0.8f);
          character.ApplyForce(character.ForwardVector * 10f);
          Script.Wait(100);
          character.ApplyForce(character.ForwardVector * 2f);
          Function.Call(Hash._0xCB2D4AB84A19AA7C, (InputArgument) this.rop);
          Function.Call(Hash._0xFFF3A50779EFBBB3, (InputArgument) this.rop);
          Game.Player.Character.Task.PlayAnimation("missrappel", "rappel_idle", 4f, 1f, -1, AnimationFlags.StayInEndFrame, 0.0f);
        }
      }
    }

    public void OnKeyDown(object sender, KeyEventArgs e)
    {
    }

    public void OnKeyUp()
    {
      Ped character = Game.Player.Character;
      if ((!this.ropeOn || !Game.IsControlJustReleased(2, GTA.Control.MoveUpOnly)) && (!this.ropeOn || !Game.IsControlJustReleased(2, GTA.Control.MoveDownOnly)) && (!this.ropeOn || !Game.IsControlJustReleased(2, GTA.Control.Attack)))
        return;
      if (Function.Call<bool>(Hash._0x1F0B79228E461EC9, (InputArgument) character, (InputArgument) "missrappel", (InputArgument) "rope_slide", (InputArgument) 1))
      {
        character.Task.ClearAll();
        Function.Call(Hash._0x538D1179EC1AA9A9, (InputArgument) this.rop);
        this.rplength = this.rop.Length + 6f;
        Function.Call(Hash._0xD009F759A723DB1B, (InputArgument) this.rop, (InputArgument) this.rplength);
        this.rop.DetachEntity((Entity) character);
        Script.Wait(10);
        this.lefthand = new Vector3(character.GetBoneCoord(Bone.IK_L_Hand).X + character.ForwardVector.X * 0.23f, character.GetBoneCoord(Bone.IK_L_Hand).Y + character.ForwardVector.Y * 0.23f, character.GetBoneCoord(Bone.IK_L_Hand).Z);
        this.rop.AttachEntities((Entity) character, this.lefthand, (Entity) this.prop, this.prop.Position, this.distance);
      }
      Game.Player.Character.Task.ClearSecondary();
      Game.Player.Character.Task.PlayAnimation("missrappel", "rappel_idle", 4f, 1f, -1, AnimationFlags.StayInEndFrame, 0.0f);
      Function.Call(Hash._0xCB2D4AB84A19AA7C, (InputArgument) this.rop);
      Function.Call(Hash._0xFFF3A50779EFBBB3, (InputArgument) this.rop);
    }

    public void OnKeyUp(object sender, KeyEventArgs e)
    {
    }
  }
}
