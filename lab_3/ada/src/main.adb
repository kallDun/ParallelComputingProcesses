with Ada.Text_IO, GNAT.Semaphores;
use Ada.Text_IO, GNAT.Semaphores;
with Ada.Containers.Indefinite_Doubly_Linked_Lists;
use Ada.Containers;

procedure Main is

   package String_Lists is new Indefinite_Doubly_Linked_Lists (String);
   use String_Lists;


   procedure Init (StorageSize : in Integer; WorkTarget : in Integer;
                   ProducersCount : in Integer; ConsumersCount : in Integer) is
      Storage : List;
      Access_Storage : Counting_Semaphore (1, Default_Ceiling);
      Full_Storage   : Counting_Semaphore (StorageSize, Default_Ceiling);
      Empty_Storage  : Counting_Semaphore (0, Default_Ceiling);
      ProducersWorkDone : Integer := 0;
      ConsumersWorkDone : Integer := 0;
      SaveRelease : Boolean := False;

      task type ProducerTask;
      task body ProducerTask is
      begin

         while True loop

            Full_Storage.Seize;
            --delay 0.25;
            Access_Storage.Seize;

            if ProducersWorkDone >= WorkTarget then
               Full_Storage.Release;
               if SaveRelease = False and Storage.Length = 0 and ConsumersWorkDone >= WorkTarget then
                  SaveRelease := true;
                  Empty_Storage.Release;
               end if;
               Access_Storage.Release;
               exit;
            end if;

            Storage.Append ("item " & ProducersWorkDone'Img);
            Put_Line ("Producer add item " & ProducersWorkDone'Img);
            ProducersWorkDone := ProducersWorkDone + 1;

            Access_Storage.Release;
            Empty_Storage.Release;

         end loop;

      end ProducerTask;


      task type ConsumerTask;
      task body ConsumerTask is
      begin

         while True loop

            Empty_Storage.Seize;
            --delay 0.25;
            Access_Storage.Seize;

            if ConsumersWorkDone >= WorkTarget then
               Empty_Storage.Release;
               if SaveRelease = False and Integer(Storage.Length) = StorageSize and ProducersWorkDone >= WorkTarget then
                  SaveRelease := true;
                  Full_Storage.Release;
               end if;
               Access_Storage.Release;
               exit;
            end if;

            declare
               item : String := First_Element (Storage);
            begin
               Put_Line ("Consumer took " & item);
            end;
            Storage.Delete_First;
            ConsumersWorkDone := ConsumersWorkDone + 1;

            Access_Storage.Release;
            Full_Storage.Release;

         end loop;

      end ConsumerTask;

      Consumers : Array(1..ConsumersCount) of ConsumerTask;
      Producers : Array(1..ProducersCount) of ProducerTask;

   begin
      null;
   end Init;

begin
   Init(4, 10, 4, 6);
end Main;
