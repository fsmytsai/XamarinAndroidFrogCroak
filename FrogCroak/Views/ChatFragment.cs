using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using FrogCroak.MyMethod;
using Android.Database;
using FrogCroak.Models;
using Java.IO;
using Android.Graphics;
using Java.Lang;
using Android.Content.PM;
using FrogCroak.Services;
using System.Threading.Tasks;

namespace FrogCroak.Views
{
    public class ChatFragment : Fragment
    {
        private RecyclerView rv_MessageList;
        private EditText et_Message;
        private MessageListAdapter messageListAdapter;
        private MyDBHelper helper;
        private ICursor cursor;

        private MainActivity mainActivity;
        private List<ChatMessage> messageList;

        private bool isFirstLoad = true;
        private bool isLoading = true;
        private bool isFinishLoad = false;

        private const int REQUEST_EXTERNAL_STORAGE = 18;
        private FileChooser fileChooser;
        private ChatService chatService;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.ChatFragment, container, false);
            mainActivity = (MainActivity)Activity;
            helper = new MyDBHelper(mainActivity, "frog.db", null, 1);

            if (mainActivity.sp_Settings.GetBoolean("IsFirst", true))
            {
                ContentValues values = new ContentValues();
                values.Put("message", "目前所有功能已提供以下10種青蛙之辨識：\n1. 艾氏樹蛙\n2. 拉都希氏赤蛙\n3. 虎皮蛙\n4. 豎琴蛙\n5. 小雨蛙\n6. 台北樹蛙\n7. 布氏樹蛙\n8. 面天樹蛙\n9. 貢德氏赤蛙\n10. 澤蛙\n\n智慧問答提供以下5種諮詢類別：\n1.介紹\n2.叫聲\n3.分布\n4.繁殖期\n5.外觀\n\n智慧問答例句：台北樹蛙的分布");
                values.Put("isme", 0);
                values.Put("type", 0);
                long id = helper.WritableDatabase.Insert("chat", null, values);
                mainActivity.sp_Settings.Edit().PutBoolean("IsFirst", false).Apply();
            }

            cursor = helper.ReadableDatabase.Query(
                "chat", null,
                null, null,
                null, null, "_id DESC");
            chatService = new ChatService();
            messageList = new List<ChatMessage>();
            initView(view);

            SetCache();

            ReadData();
            return view;
        }

        private void initView(View view)
        {
            rv_MessageList = (RecyclerView)view.FindViewById(Resource.Id.rv_MessageList);
            et_Message = (EditText)view.FindViewById(Resource.Id.et_Message);

            ImageButton ib_SendMessage = (ImageButton)view.FindViewById(Resource.Id.ib_SendMessage);
            ib_SendMessage.Click += delegate
            {
                SendMessage();
            };


            ImageButton ib_SendImage = (ImageButton)view.FindViewById(Resource.Id.ib_SendImage);
            ib_SendImage.Click += delegate
            {
                SendImage();
            };

        }

        MyLruCache lruCache;

        public void SetCache()
        {
            var maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / 5);
            lruCache = new MyLruCache(maxMemory);
        }


        private int count = 0;

        private void ReadData()
        {
            while (cursor.MoveToNext())
            {
                count++;
                string Message = cursor.GetString(1);
                messageList.Add(new ChatMessage(
                        cursor.GetInt(0),
                        Message,
                        cursor.GetInt(2),
                        cursor.GetInt(3))
                );

                if (cursor.GetInt(3) != 0)
                {
                    if (lruCache.Get(Message) == null)
                    {
                        Bitmap bitmap = BitmapFactory.DecodeFile(mainActivity.FilesDir.AbsolutePath + "/" + Message);
                        lruCache.Put(Message, bitmap);
                    }
                }

                if (count % 10 == 0)
                    break;
            }
            isFinishLoad = cursor.IsAfterLast;
            isLoading = false;

            if (isFirstLoad)
            {
                isFirstLoad = false;
                rv_MessageList.SetLayoutManager(new WrapContentLinearLayoutManager(mainActivity, LinearLayoutManager.Vertical, true));
                messageListAdapter = new MessageListAdapter(this);
                rv_MessageList.SetAdapter(messageListAdapter);
            }
            else
            {
                messageListAdapter.NotifyDataSetChanged();
            }
        }

        public class MessageListAdapter : RecyclerView.Adapter
        {
            private ChatFragment chatFragment;

            private readonly int left = 87;
            private readonly int right = 78;

            public MessageListAdapter(ChatFragment chatFragment)
            {
                this.chatFragment = chatFragment;
            }

            public override int GetItemViewType(int position)
            {
                if (chatFragment.messageList[position].from == 0)
                    return left;
                else
                    return right;
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                Context context = parent.Context;
                View view;
                if (viewType == right)
                {
                    view = LayoutInflater.From(context).Inflate(Resource.Layout.RMessageBlock, parent, false);
                }
                else
                {
                    view = LayoutInflater.From(context).Inflate(Resource.Layout.LMessageBlock, parent, false);
                }
                ViewHolder viewHolder = new ViewHolder(view);
                return viewHolder;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                ViewHolder vh = holder as ViewHolder;

                if (chatFragment.messageList[position].type == 0)
                {
                    vh.tv_Message.Visibility = ViewStates.Visible;
                    if (GetItemViewType(position) == right)
                        vh.iv_Frog.Visibility = ViewStates.Gone;
                    vh.tv_Message.Text = chatFragment.messageList[position].message;
                }
                else
                {
                    vh.tv_Message.Visibility = ViewStates.Gone;
                    vh.iv_Frog.Visibility = ViewStates.Visible;
                    Bitmap bitmap = (Bitmap)chatFragment.lruCache.Get(chatFragment.messageList[position].message);
                    vh.iv_Frog.SetImageBitmap(bitmap);
                }

                //避免重複請求
                if (position > chatFragment.messageList.Count * 0.6 && !chatFragment.isFinishLoad && !chatFragment.isLoading)
                {
                    chatFragment.isLoading = true;
                    chatFragment.ReadData();
                }
            }

            public override int ItemCount => chatFragment.messageList.Count;

            public class ViewHolder : RecyclerView.ViewHolder
            {
                public TextView tv_Message { get; private set; }
                public ImageView iv_Frog { get; private set; }

                public ViewHolder(View itemView)
                        : base(itemView)
                {
                    tv_Message = (TextView)itemView.FindViewById(Resource.Id.tv_Message);
                    iv_Frog = (ImageView)itemView.FindViewById(Resource.Id.iv_Frog);
                }
            }
        }

        private async void SendMessage()
        {
            SharedService.HideKeyboard(mainActivity);
            mainActivity.activity_Outer.RequestFocus();

            string Message = et_Message.Text;

            if (Message.Trim() != "")
            {
                if (SharedService.CheckNetWork(mainActivity))
                {
                    et_Message.Text = "";
                    ContentValues values = new ContentValues();
                    values.Put("message", Message);
                    values.Put("isme", 1);
                    values.Put("type", 0);
                    long id = helper.WritableDatabase.Insert("chat", null, values);

                    messageList.Insert(0, new ChatMessage(
                            (int)id,
                            Message,
                            1,
                            0)
                    );

                    messageListAdapter.NotifyItemInserted(0);
                    rv_MessageList.ScrollToPosition(0);
                }
                AllRequestResult result = null;
                await Task.Run(async () =>
                {
                    result = await chatService.CreateMessage(Message);
                });
                if (result.IsSuccess)
                {
                    string message = (string)result.Result;
                    message = message.Replace("\\n", "\n");
                    ContentValues values = new ContentValues();
                    values.Put("message", message);
                    values.Put("isme", 0);
                    values.Put("type", 0);
                    long id = helper.WritableDatabase.Insert("chat", null, values);

                    messageList.Insert(0, new ChatMessage(
                            (int)id,
                            message,
                            0,
                            0)
                    );

                    messageListAdapter.NotifyItemInserted(0);
                    rv_MessageList.ScrollToPosition(0);
                }
                else
                {
                    SharedService.ShowTextToast((string)result.Result, mainActivity);
                }
            }
            else
            {
                SharedService.ShowTextToast("請輸入內容", mainActivity);
            }
        }

        private void SendImage()
        {
            if (ActivityCompat.CheckSelfPermission(mainActivity, Android.Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
            {
                fileChooser = new FileChooser(mainActivity, this);
                if (!fileChooser.showFileChooser("image/*", null, false, true))
                {
                    SharedService.ShowTextToast("您沒有適合的檔案選取器", mainActivity);
                }
            }
            else
            {
                RequestPermissions(new string[] { Android.Manifest.Permission.ReadExternalStorage }, REQUEST_EXTERNAL_STORAGE);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            switch (requestCode)
            {
                case REQUEST_EXTERNAL_STORAGE:
                    if (grantResults.Length > 0 && grantResults[0] == (int)Permission.Granted)
                    {
                        fileChooser = new FileChooser(mainActivity, this);
                        if (!fileChooser.showFileChooser("image/*", null, false, true))
                        {
                            SharedService.ShowTextToast("您沒有適合的檔案選取器", mainActivity);
                        }
                    }
                    else
                    {
                        SharedService.ShowTextToast("您拒絕選取檔案", mainActivity);
                    }
                    return;
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == FileChooser.ACTIVITY_FILE_CHOOSER)
            {
                if (fileChooser.onActivityResult(requestCode, resultCode, data))
                {
                    File[] files = fileChooser.getChosenFiles();
                    UploadImg(files);
                }
            }
        }

        private async void UploadImg(File[] files)
        {
            if (SharedService.CheckNetWork(mainActivity))
            {
                if (lruCache.Get(files[0].Name) == null)
                {
                    SharedService.ShowTextToast("壓縮圖片中...", mainActivity);
                    Bitmap bitmap = null;
                    await Task.Run(() =>
                    {
                        bitmap = SharedService.CompressImage(files[0].AbsolutePath, mainActivity.FilesDir.AbsolutePath + "/" + files[0].Name);
                    });

                    if (bitmap == null)
                    {
                        SharedService.ShowTextToast("壓縮圖片失敗QQ", mainActivity);
                        return;
                    }
                    lruCache.Put(files[0].Name, bitmap);
                }

                ContentValues values = new ContentValues();
                values.Put("message", files[0].Name);
                values.Put("isme", 1);
                values.Put("type", 1);
                long id = helper.WritableDatabase.Insert("chat", null, values);

                messageList.Insert(0, new ChatMessage(
                        (int)id,
                        files[0].Name,
                        1,
                        1)
                );

                messageListAdapter.NotifyItemRangeInserted(0, 1);
                rv_MessageList.ScrollToPosition(0);

                SharedService.ShowTextToast("圖片上傳中...", mainActivity);
                string FileName = files[0].Name;
                string[] Type = FileName.Split('.');
                if (System.String.Compare(Type[Type.Length - 1], "jpg", true) == 0)
                    Type[Type.Length - 1] = "jpeg";

                AllRequestResult result = null;

                await Task.Run(async () =>
                {
                    result = await chatService.UploadImage(mainActivity.FilesDir.AbsolutePath + "/" + files[0].Name, Type[Type.Length - 1]);
                });

                if (result.IsSuccess)
                {
                    values = new ContentValues();
                    values.Put("message", (string)result.Result);
                    values.Put("isme", 0);
                    values.Put("type", 0);
                    id = helper.WritableDatabase.Insert("chat", null, values);

                    messageList.Insert(0, new ChatMessage(
                            (int)id,
                            (string)result.Result,
                            0,
                            0)
                    );

                    messageListAdapter.NotifyItemRangeInserted(0, 1);
                    rv_MessageList.ScrollToPosition(0);
                }
                else
                {
                    SharedService.ShowTextToast((string)result.Result, mainActivity);
                }

            }
        }

        public class WrapContentLinearLayoutManager : LinearLayoutManager
        {
            public WrapContentLinearLayoutManager(Context context, int orientation, bool reverseLayout)
                    : base(context, orientation, reverseLayout)
            {
            }

            public override void OnLayoutChildren(RecyclerView.Recycler recycler, RecyclerView.State state)
            {
                try
                {
                    base.OnLayoutChildren(recycler, state);
                }
                catch (IndexOutOfBoundsException e)
                {
                    Log.Error("probe", "meet a IOOBE in RecyclerView");
                }
            }
        }
    }
}