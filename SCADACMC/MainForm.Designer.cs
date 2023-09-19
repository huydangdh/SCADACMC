namespace SCADACMC
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("SCADA_CENTER");
            this.tmrCheckStatus = new System.Windows.Forms.Timer(this.components);
            this.ctxTool = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemInspObj = new System.Windows.Forms.ToolStripMenuItem();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lblAlertCounter = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.trvCenter = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tbxRecvData = new System.Windows.Forms.RichTextBox();
            this.toolBar = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolBtnStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.cN51MODEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLogFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.storageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.poolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tmrCheckMaterialOver10Minds = new System.Windows.Forms.Timer(this.components);
            this.tmrCheckLossMaterial = new System.Windows.Forms.Timer(this.components);
            this.winsock1 = new Treorisoft.Net.Winsock();
            this.ctxTool.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tmrCheckStatus
            // 
            this.tmrCheckStatus.Interval = 15000;
            this.tmrCheckStatus.Tick += new System.EventHandler(this.tmrCheckStatus_Tick);
            // 
            // ctxTool
            // 
            this.ctxTool.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemInspObj});
            this.ctxTool.Name = "ctxTool";
            this.ctxTool.Size = new System.Drawing.Size(154, 26);
            // 
            // itemInspObj
            // 
            this.itemInspObj.Name = "itemInspObj";
            this.itemInspObj.Size = new System.Drawing.Size(153, 22);
            this.itemInspObj.Text = "InspThisObject";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(456, 32);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(88, 30);
            this.button3.TabIndex = 13;
            this.button3.Text = "RESET";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(362, 32);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 30);
            this.button2.TabIndex = 12;
            this.button2.Text = "GCCollector";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(267, 32);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 30);
            this.button1.TabIndex = 11;
            this.button1.Text = "TestAlarm";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // lblAlertCounter
            // 
            this.lblAlertCounter.AutoSize = true;
            this.lblAlertCounter.BackColor = System.Drawing.Color.Transparent;
            this.lblAlertCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.lblAlertCounter.ForeColor = System.Drawing.Color.Red;
            this.lblAlertCounter.Location = new System.Drawing.Point(236, 25);
            this.lblAlertCounter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAlertCounter.Name = "lblAlertCounter";
            this.lblAlertCounter.Size = new System.Drawing.Size(14, 13);
            this.lblAlertCounter.TabIndex = 10;
            this.lblAlertCounter.Text = "0";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "000_Abort_h32bit_16.png");
            this.imageList1.Images.SetKeyName(1, "000_AddApplicationServer_h32bit_16.png");
            this.imageList1.Images.SetKeyName(2, "000_AddApplicationServer_h32bit_24.png");
            this.imageList1.Images.SetKeyName(3, "000_AddIPAddress_h32bit_16.png");
            this.imageList1.Images.SetKeyName(4, "000_AddSite_h32bit_16.png");
            this.imageList1.Images.SetKeyName(5, "000_Alert2_h32bit_16.png");
            this.imageList1.Images.SetKeyName(6, "000_BackupMetadata_h32bit_16.png");
            this.imageList1.Images.SetKeyName(7, "000_BackupMetadata_h32bit_32.png");
            this.imageList1.Images.SetKeyName(8, "000_ConfigureIPAddresses_h32bit_16.png");
            this.imageList1.Images.SetKeyName(9, "000_CPU_h32bit_16.png");
            this.imageList1.Images.SetKeyName(10, "000_CreateVirtualStorage_h32bit_32.png");
            this.imageList1.Images.SetKeyName(11, "000_CreateVM_h32bit_24.png");
            this.imageList1.Images.SetKeyName(12, "000_CreateVM_h32bit_32.png");
            this.imageList1.Images.SetKeyName(13, "000_defaultSpyglass_h32bit_16.png");
            this.imageList1.Images.SetKeyName(14, "000_DeleteAllMessages_h32bit_16.png");
            this.imageList1.Images.SetKeyName(15, "000_DeleteMessage_h32bit_16.png");
            this.imageList1.Images.SetKeyName(16, "000_DeleteVirtualAppliance_h32bit_16.png");
            this.imageList1.Images.SetKeyName(17, "000_DisasterRecovery_h32bit_32.png");
            this.imageList1.Images.SetKeyName(18, "000_Email_h32bit_16.png");
            this.imageList1.Images.SetKeyName(19, "000_EnablePowerControl_h32bit_16.png");
            this.imageList1.Images.SetKeyName(20, "000_error_h32bit_16.png");
            this.imageList1.Images.SetKeyName(21, "000_error_h32bit_32.png");
            this.imageList1.Images.SetKeyName(22, "000_ExcludeHost_h32bit_16.png");
            this.imageList1.Images.SetKeyName(23, "000_ExportMessages_h32bit_16.png");
            this.imageList1.Images.SetKeyName(24, "000_ExportVirtualAppliance_h32bit_16.png");
            this.imageList1.Images.SetKeyName(25, "000_ExportVirtualAppliance_h32bit_32.png");
            this.imageList1.Images.SetKeyName(26, "000_Failback_h32bit_32.png");
            this.imageList1.Images.SetKeyName(27, "000_Failover_h32bit_32.png");
            this.imageList1.Images.SetKeyName(28, "000_Fields_h32bit_16.png");
            this.imageList1.Images.SetKeyName(29, "000_FilterDates_h32bit_16.png");
            this.imageList1.Images.SetKeyName(30, "000_FilterServer_h32bit_16.png");
            this.imageList1.Images.SetKeyName(31, "000_FilterSeverity_h32bit_16.png");
            this.imageList1.Images.SetKeyName(32, "000_Folder_open_h32bit_16.png");
            this.imageList1.Images.SetKeyName(33, "000_GetMemoryInfo_h32bit_16.png");
            this.imageList1.Images.SetKeyName(34, "000_GetMemoryInfo_h32bit_32.png");
            this.imageList1.Images.SetKeyName(35, "000_GetServerReport_h32bit_16.png");
            this.imageList1.Images.SetKeyName(36, "000_GetServerReport_h32bit_32.png");
            this.imageList1.Images.SetKeyName(37, "000_HAServer_h32bit_32.png");
            this.imageList1.Images.SetKeyName(38, "000_HelpIM_h32bit_16.png");
            this.imageList1.Images.SetKeyName(39, "000_HelpIM_h32bit_32.png");
            this.imageList1.Images.SetKeyName(40, "000_HighlightVM_h32bit_24.png");
            this.imageList1.Images.SetKeyName(41, "000_HighLightVM_h32bit_32.png");
            this.imageList1.Images.SetKeyName(42, "000_host_0_star.png");
            this.imageList1.Images.SetKeyName(43, "000_host_1_star.png");
            this.imageList1.Images.SetKeyName(44, "000_host_2_star.png");
            this.imageList1.Images.SetKeyName(45, "000_host_3_star.png");
            this.imageList1.Images.SetKeyName(46, "000_host_4_star.png");
            this.imageList1.Images.SetKeyName(47, "000_host_5_star.png");
            this.imageList1.Images.SetKeyName(48, "000_host_6_star.png");
            this.imageList1.Images.SetKeyName(49, "000_host_7_star.png");
            this.imageList1.Images.SetKeyName(50, "000_host_8_star.png");
            this.imageList1.Images.SetKeyName(51, "000_host_9_star.png");
            this.imageList1.Images.SetKeyName(52, "000_host_10_star.png");
            this.imageList1.Images.SetKeyName(53, "000_HostUnpatched_h32bit_16.png");
            this.imageList1.Images.SetKeyName(54, "000_ImportVirtualAppliance_h32bit_16.png");
            this.imageList1.Images.SetKeyName(55, "000_ImportVirtualAppliance_h32bit_32.png");
            this.imageList1.Images.SetKeyName(56, "000_ImportVM_h32bit_32.png");
            this.imageList1.Images.SetKeyName(57, "000_Info3_h32bit_16.png");
            this.imageList1.Images.SetKeyName(58, "000_ManagementInterface_h32bit_16.png");
            this.imageList1.Images.SetKeyName(59, "000_MigrateStoppedVM_h32bit_16.png");
            this.imageList1.Images.SetKeyName(60, "000_MigrateSuspendedVM_h32bit_16.png");
            this.imageList1.Images.SetKeyName(61, "000_MigrateVM_h32bit_16.png");
            this.imageList1.Images.SetKeyName(62, "000_MigrateVM_h32bit_32.png");
            this.imageList1.Images.SetKeyName(63, "000_Module_h32bit_16.png");
            this.imageList1.Images.SetKeyName(64, "000_Network_h32bit_16.png");
            this.imageList1.Images.SetKeyName(65, "000_NewNetwork_h32bit_32.png");
            this.imageList1.Images.SetKeyName(66, "000_NewStorage_h32bit_16.png");
            this.imageList1.Images.SetKeyName(67, "000_NewStorage_h32bit_24.png");
            this.imageList1.Images.SetKeyName(68, "000_NewStorage_h32bit_32.png");
            this.imageList1.Images.SetKeyName(69, "000_NewVirtualAppliance_h32bit_16.png");
            this.imageList1.Images.SetKeyName(70, "000_NewVirtualAppliance_h32bit_32.png");
            this.imageList1.Images.SetKeyName(71, "000_Optimize_h32bit_16.png");
            this.imageList1.Images.SetKeyName(72, "000_Patch_h32bit_16.png");
            this.imageList1.Images.SetKeyName(73, "000_Patch_h32bit_32.png");
            this.imageList1.Images.SetKeyName(74, "000_paused_h32bit_16.png");
            this.imageList1.Images.SetKeyName(75, "000_paused_h32bit_16_green.png");
            this.imageList1.Images.SetKeyName(76, "000_Paused_h32bit_24.png");
            this.imageList1.Images.SetKeyName(77, "000_Paused_h32bit_24_green.png");
            this.imageList1.Images.SetKeyName(78, "000_Pool_h32bit_16.png");
            this.imageList1.Images.SetKeyName(79, "000_Pool_h32bit_16-w-alert.png");
            this.imageList1.Images.SetKeyName(80, "000_PoolConnected_h32bit_16.png");
            this.imageList1.Images.SetKeyName(81, "000_PoolNew_h32bit_16.png");
            this.imageList1.Images.SetKeyName(82, "000_PoolNew_h32bit_24.png");
            this.imageList1.Images.SetKeyName(83, "000_RebootVM_h32bit_16.png");
            this.imageList1.Images.SetKeyName(84, "000_RemoveIPAddress_h32bit_16.png");
            this.imageList1.Images.SetKeyName(85, "000_RemoveSite_h32bit_16.png");
            this.imageList1.Images.SetKeyName(86, "000_Resumed_h32bit_16.png");
            this.imageList1.Images.SetKeyName(87, "000_Resumed_h32bit_16_green.png");
            this.imageList1.Images.SetKeyName(88, "000_Resumed_h32bit_24.png");
            this.imageList1.Images.SetKeyName(89, "000_Resumed_h32bit_24_green.png");
            this.imageList1.Images.SetKeyName(90, "000_ScheduledVMsnapshotDiskMemory_h32bit_16.png");
            this.imageList1.Images.SetKeyName(91, "000_ScheduledVMSnapshotDiskMemory_h32bit_32.png");
            this.imageList1.Images.SetKeyName(92, "000_ScheduledVMsnapshotDiskOnly_h32bit_16.png");
            this.imageList1.Images.SetKeyName(93, "000_ScheduledVMsnapshotDiskOnly_h32bit_32.png");
            this.imageList1.Images.SetKeyName(94, "000_Search_h32bit_16.png");
            this.imageList1.Images.SetKeyName(95, "000_Server_h32bit_16.png");
            this.imageList1.Images.SetKeyName(96, "000_Server_h32bit_16-w-alert.png");
            this.imageList1.Images.SetKeyName(97, "000_ServerDisconnected_h32bit_16.png");
            this.imageList1.Images.SetKeyName(98, "000_ServerErrorFile_h32bit_16.png");
            this.imageList1.Images.SetKeyName(99, "000_ServerHome_h32bit_16.png");
            this.imageList1.Images.SetKeyName(100, "000_ServerInProgress_h32bit_16.png");
            this.imageList1.Images.SetKeyName(101, "000_ServerMaintenance_h32bit_16.png");
            this.imageList1.Images.SetKeyName(102, "000_ServerMaintenance_h32bit_32.png");
            this.imageList1.Images.SetKeyName(103, "000_ServerWlb_h32bit_16.png");
            this.imageList1.Images.SetKeyName(104, "000_Sites_h32bit_16.png");
            this.imageList1.Images.SetKeyName(105, "000_SliderTexture.png");
            this.imageList1.Images.SetKeyName(106, "000_StartVM_h32bit_16.png");
            this.imageList1.Images.SetKeyName(107, "000_StoppedVM_h32bit_16.png");
            this.imageList1.Images.SetKeyName(108, "000_Storage_h32bit_16.png");
            this.imageList1.Images.SetKeyName(109, "000_StorageBroken_h32bit_16.png");
            this.imageList1.Images.SetKeyName(110, "000_StorageDefault_h32bit_16.png");
            this.imageList1.Images.SetKeyName(111, "000_StorageDisabled_h32bit_16.png");
            this.imageList1.Images.SetKeyName(112, "000_SuspendVM_h32bit_16.png");
            this.imageList1.Images.SetKeyName(113, "000_SuspendVM_h32bit_16_green.png");
            this.imageList1.Images.SetKeyName(114, "000_SwitcherBackground.png");
            this.imageList1.Images.SetKeyName(115, "000_Tag_h32bit_16.png");
            this.imageList1.Images.SetKeyName(116, "000_TCP-IPGroup_h32bit_16.png");
            this.imageList1.Images.SetKeyName(117, "000_TemplateDisabled_h32bit_16.png");
            this.imageList1.Images.SetKeyName(118, "000_TestFailover_h32bit_32.png");
            this.imageList1.Images.SetKeyName(119, "000_Tick_h32bit_16.png");
            this.imageList1.Images.SetKeyName(120, "000_ToolBar_Pref_Icon_dis.png");
            this.imageList1.Images.SetKeyName(121, "000_ToolBar_Pref_Icon_ovr.png");
            this.imageList1.Images.SetKeyName(122, "000_ToolBar_Pref_Icon_up.png");
            this.imageList1.Images.SetKeyName(123, "000_ToolBar_USB_Icon_dis.png");
            this.imageList1.Images.SetKeyName(124, "000_ToolBar_USB_Icon_ovr.png");
            this.imageList1.Images.SetKeyName(125, "000_ToolBar_USB_Icon_up.png");
            this.imageList1.Images.SetKeyName(126, "000_TreeConnected_h32bit_16.png");
            this.imageList1.Images.SetKeyName(127, "000_UpgradePool_h32bit_32.png");
            this.imageList1.Images.SetKeyName(128, "000_User_h32bit_16.png");
            this.imageList1.Images.SetKeyName(129, "000_user_h32bit_32.png");
            this.imageList1.Images.SetKeyName(130, "000_userandgroup_h32bit_16.png");
            this.imageList1.Images.SetKeyName(131, "000_UserAndGroup_h32bit_32.png");
            this.imageList1.Images.SetKeyName(132, "000_UserTemplate_h32bit_16.png");
            this.imageList1.Images.SetKeyName(133, "000_ViewModeList_h32bit_16.png");
            this.imageList1.Images.SetKeyName(134, "000_ViewModeTree_h32bit_16.png");
            this.imageList1.Images.SetKeyName(135, "000_VirtualAppliance_h32bit_16.png");
            this.imageList1.Images.SetKeyName(136, "000_VirtualStorage_h32bit_16.png");
            this.imageList1.Images.SetKeyName(137, "000_VM_h32bit_16.png");
            this.imageList1.Images.SetKeyName(138, "000_VM_h32bit_24.png");
            this.imageList1.Images.SetKeyName(139, "000_VMDisabled_h32bit_16.png");
            this.imageList1.Images.SetKeyName(140, "000_VMPausedDisabled_h32bit_16.png");
            this.imageList1.Images.SetKeyName(141, "000_VMPausedDisabled_h32bit_16_green.png");
            this.imageList1.Images.SetKeyName(142, "000_VMSession_h32bit_16.png");
            this.imageList1.Images.SetKeyName(143, "000_VMSnapshotDiskMemory_h32bit_16.png");
            this.imageList1.Images.SetKeyName(144, "000_VMSnapshotDiskMemory_h32bit_32.png");
            this.imageList1.Images.SetKeyName(145, "000_VMSnapShotDiskOnly_h32bit_16.png");
            this.imageList1.Images.SetKeyName(146, "000_VMSnapShotDiskOnly_h32bit_32.png");
            this.imageList1.Images.SetKeyName(147, "000_VMStarting_h32bit_16.png");
            this.imageList1.Images.SetKeyName(148, "000_VMStartingDisabled_h32bit_16.png");
            this.imageList1.Images.SetKeyName(149, "000_VMStoppedDisabled_h32bit_16.png");
            this.imageList1.Images.SetKeyName(150, "000_VMTemplate_h32bit_16.png");
            this.imageList1.Images.SetKeyName(151, "000_WarningAlert_h32bit_32.png");
            this.imageList1.Images.SetKeyName(152, "000_weighting_h32bit_16.png");
            this.imageList1.Images.SetKeyName(153, "000_XenCenterAlerts_h32bit_24.png");
            this.imageList1.Images.SetKeyName(154, "001_Back_h32bit_24.png");
            this.imageList1.Images.SetKeyName(155, "001_CreateVM_h32bit_16.png");
            this.imageList1.Images.SetKeyName(156, "001_ForceReboot_h32bit_16.png");
            this.imageList1.Images.SetKeyName(157, "001_ForceReboot_h32bit_24.png");
            this.imageList1.Images.SetKeyName(158, "001_ForceShutDown_h32bit_16.png");
            this.imageList1.Images.SetKeyName(159, "001_ForceShutDown_h32bit_24.png");
            this.imageList1.Images.SetKeyName(160, "001_Forward_h32bit_24.png");
            this.imageList1.Images.SetKeyName(161, "001_LifeCycle_h32bit_24.png");
            this.imageList1.Images.SetKeyName(162, "001_PerformanceGraph_h32bit_16.png");
            this.imageList1.Images.SetKeyName(163, "001_Pin_h32bit_16.png");
            this.imageList1.Images.SetKeyName(164, "001_PowerOn_h32bit_16.png");
            this.imageList1.Images.SetKeyName(165, "001_PowerOn_h32bit_24.png");
            this.imageList1.Images.SetKeyName(166, "001_Reboot_h32bit_16.png");
            this.imageList1.Images.SetKeyName(167, "001_Reboot_h32bit_24.png");
            this.imageList1.Images.SetKeyName(168, "001_ShutDown_h32bit_16.png");
            this.imageList1.Images.SetKeyName(169, "001_ShutDown_h32bit_24.png");
            this.imageList1.Images.SetKeyName(170, "001_Tools_h32bit_16.png");
            this.imageList1.Images.SetKeyName(171, "001_WindowView_h32bit_16.png");
            this.imageList1.Images.SetKeyName(172, "002_Configure_h32bit_16.png");
            this.imageList1.Images.SetKeyName(173, "015_Download_h32bit_32.png");
            this.imageList1.Images.SetKeyName(174, "075_TickRound_h32bit_16.png");
            this.imageList1.Images.SetKeyName(175, "075_WarningRound_h32bit_16.png");
            this.imageList1.Images.SetKeyName(176, "112_LeftArrowLong_Blue_24x24_72.png");
            this.imageList1.Images.SetKeyName(177, "112_RightArrowLong_Blue_24x24_72.png");
            this.imageList1.Images.SetKeyName(178, "about_box_graphic_423x79.png");
            this.imageList1.Images.SetKeyName(179, "ajax-loader.gif");
            this.imageList1.Images.SetKeyName(180, "alert1_16.png");
            this.imageList1.Images.SetKeyName(181, "alert2_16.png");
            this.imageList1.Images.SetKeyName(182, "alert3_16.png");
            this.imageList1.Images.SetKeyName(183, "alert4_16.png");
            this.imageList1.Images.SetKeyName(184, "alert5_16.png");
            this.imageList1.Images.SetKeyName(185, "alert6_16.png");
            this.imageList1.Images.SetKeyName(186, "alerts_32.png");
            this.imageList1.Images.SetKeyName(187, "ascending_triangle.png");
            this.imageList1.Images.SetKeyName(188, "asianux_16x.png");
            this.imageList1.Images.SetKeyName(189, "asterisk.png");
            this.imageList1.Images.SetKeyName(190, "attach_24.png");
            this.imageList1.Images.SetKeyName(191, "attach_virtualstorage_32.png");
            this.imageList1.Images.SetKeyName(192, "backup_restore_32.png");
            this.imageList1.Images.SetKeyName(193, "cancelled_action_16.png");
            this.imageList1.Images.SetKeyName(194, "centos_16x.png");
            this.imageList1.Images.SetKeyName(195, "change_password_16.png");
            this.imageList1.Images.SetKeyName(196, "change_password_32.png");
            this.imageList1.Images.SetKeyName(197, "clonevm_16.png");
            this.imageList1.Images.SetKeyName(198, "close_16.png");
            this.imageList1.Images.SetKeyName(199, "commands_16.png");
            this.imageList1.Images.SetKeyName(200, "console_16.png");
            this.imageList1.Images.SetKeyName(201, "contracted_triangle.png");
            this.imageList1.Images.SetKeyName(202, "copy_16.png");
            this.imageList1.Images.SetKeyName(203, "coreos-16.png");
            this.imageList1.Images.SetKeyName(204, "coreos-globe-icon.png");
            this.imageList1.Images.SetKeyName(205, "cross.png");
            this.imageList1.Images.SetKeyName(206, "DateTime16.png");
            this.imageList1.Images.SetKeyName(207, "DC_16.png");
            this.imageList1.Images.SetKeyName(208, "debian_16x.png");
            this.imageList1.Images.SetKeyName(209, "descending_triangle.png");
            this.imageList1.Images.SetKeyName(210, "desktop.jpg");
            this.imageList1.Images.SetKeyName(211, "detach_24.png");
            this.imageList1.Images.SetKeyName(212, "edit_16.png");
            this.imageList1.Images.SetKeyName(213, "expanded_triangle.png");
            this.imageList1.Images.SetKeyName(214, "export_32.png");
            this.imageList1.Images.SetKeyName(215, "folder_grey.png");
            this.imageList1.Images.SetKeyName(216, "folder_separator.png");
            this.imageList1.Images.SetKeyName(217, "grab.png");
            this.imageList1.Images.SetKeyName(218, "grapharea.png");
            this.imageList1.Images.SetKeyName(219, "graphline.png");
            this.imageList1.Images.SetKeyName(220, "gripper.png");
            this.imageList1.Images.SetKeyName(221, "ha_16.png");
            this.imageList1.Images.SetKeyName(222, "help_16_hover.png");
            this.imageList1.Images.SetKeyName(223, "help_24.png");
            this.imageList1.Images.SetKeyName(224, "help_24_hover.png");
            this.imageList1.Images.SetKeyName(225, "help_32_hover.png");
            this.imageList1.Images.SetKeyName(226, "homepage_bullet.png");
            this.imageList1.Images.SetKeyName(227, "import_32.png");
            this.imageList1.Images.SetKeyName(228, "infra_view_16.png");
            this.imageList1.Images.SetKeyName(229, "infra_view_16_textured.png");
            this.imageList1.Images.SetKeyName(230, "infra_view_24.png");
            this.imageList1.Images.SetKeyName(231, "licensekey_32.png");
            this.imageList1.Images.SetKeyName(232, "lifecycle_hot.png");
            this.imageList1.Images.SetKeyName(233, "lifecycle_pressed.png");
            this.imageList1.Images.SetKeyName(234, "linx_16x.png");
            this.imageList1.Images.SetKeyName(235, "log_destination_16.png");
            this.imageList1.Images.SetKeyName(236, "Logo.png");
            this.imageList1.Images.SetKeyName(237, "memory_dynmax_slider.png");
            this.imageList1.Images.SetKeyName(238, "memory_dynmax_slider_dark.png");
            this.imageList1.Images.SetKeyName(239, "memory_dynmax_slider_light.png");
            this.imageList1.Images.SetKeyName(240, "memory_dynmax_slider_noedit.png");
            this.imageList1.Images.SetKeyName(241, "memory_dynmax_slider_noedit_small.png");
            this.imageList1.Images.SetKeyName(242, "memory_dynmax_slider_small.png");
            this.imageList1.Images.SetKeyName(243, "memory_dynmin_slider.png");
            this.imageList1.Images.SetKeyName(244, "memory_dynmin_slider_dark.png");
            this.imageList1.Images.SetKeyName(245, "memory_dynmin_slider_light.png");
            this.imageList1.Images.SetKeyName(246, "memory_dynmin_slider_noedit.png");
            this.imageList1.Images.SetKeyName(247, "memory_dynmin_slider_noedit_small.png");
            this.imageList1.Images.SetKeyName(248, "memory_dynmin_slider_small.png");
            this.imageList1.Images.SetKeyName(249, "minus.png");
            this.imageList1.Images.SetKeyName(250, "more_16.png");
            this.imageList1.Images.SetKeyName(251, "neokylin_16x.png");
            this.imageList1.Images.SetKeyName(252, "notif_alerts_16.png");
            this.imageList1.Images.SetKeyName(253, "notif_events_16.png");
            this.imageList1.Images.SetKeyName(254, "notif_events_errors_16.png");
            this.imageList1.Images.SetKeyName(255, "notif_none_16.png");
            this.imageList1.Images.SetKeyName(256, "notif_none_24.png");
            this.imageList1.Images.SetKeyName(257, "notif_updates_16.png");
            this.imageList1.Images.SetKeyName(258, "objects_16.png");
            this.imageList1.Images.SetKeyName(259, "objects_16_textured.png");
            this.imageList1.Images.SetKeyName(260, "objects_24.png");
            this.imageList1.Images.SetKeyName(261, "oracle_16x.png");
            this.imageList1.Images.SetKeyName(262, "org_view_16.png");
            this.imageList1.Images.SetKeyName(263, "org_view_24.png");
            this.imageList1.Images.SetKeyName(264, "padlock.png");
            this.imageList1.Images.SetKeyName(265, "paste_16.png");
            this.imageList1.Images.SetKeyName(266, "PausedDC_16.png");
            this.imageList1.Images.SetKeyName(267, "PDChevronDown.png");
            this.imageList1.Images.SetKeyName(268, "PDChevronDownOver.png");
            this.imageList1.Images.SetKeyName(269, "PDChevronLeft.png");
            this.imageList1.Images.SetKeyName(270, "PDChevronRight.png");
            this.imageList1.Images.SetKeyName(271, "PDChevronUp.png");
            this.imageList1.Images.SetKeyName(272, "PDChevronUpOver.png");
            this.imageList1.Images.SetKeyName(273, "pool_up_16.png");
            this.imageList1.Images.SetKeyName(274, "queued.png");
            this.imageList1.Images.SetKeyName(275, "redhat_16x.png");
            this.imageList1.Images.SetKeyName(276, "Refresh16.png");
            this.imageList1.Images.SetKeyName(277, "RunningDC_16.png");
            this.imageList1.Images.SetKeyName(278, "saved_searches_16.png");
            this.imageList1.Images.SetKeyName(279, "saved_searches_24.png");
            this.imageList1.Images.SetKeyName(280, "scilinux_16x.png");
            this.imageList1.Images.SetKeyName(281, "server_up_16.png");
            this.imageList1.Images.SetKeyName(282, "sl_16.png");
            this.imageList1.Images.SetKeyName(283, "sl_add_storage_system_16.png");
            this.imageList1.Images.SetKeyName(284, "sl_add_storage_system_32.png");
            this.imageList1.Images.SetKeyName(285, "sl_add_storage_system_small_16.png");
            this.imageList1.Images.SetKeyName(286, "sl_connected_16.png");
            this.imageList1.Images.SetKeyName(287, "sl_connecting_16.png");
            this.imageList1.Images.SetKeyName(288, "sl_disconnected_16.png");
            this.imageList1.Images.SetKeyName(289, "sl_lun_16.png");
            this.imageList1.Images.SetKeyName(290, "sl_luns_16.png");
            this.imageList1.Images.SetKeyName(291, "sl_pool_16.png");
            this.imageList1.Images.SetKeyName(292, "sl_pools_16.png");
            this.imageList1.Images.SetKeyName(293, "sl_system_16.png");
            this.imageList1.Images.SetKeyName(294, "SpinningFrame0.png");
            this.imageList1.Images.SetKeyName(295, "SpinningFrame1.png");
            this.imageList1.Images.SetKeyName(296, "SpinningFrame2.png");
            this.imageList1.Images.SetKeyName(297, "SpinningFrame3.png");
            this.imageList1.Images.SetKeyName(298, "SpinningFrame4.png");
            this.imageList1.Images.SetKeyName(299, "SpinningFrame5.png");
            this.imageList1.Images.SetKeyName(300, "SpinningFrame6.png");
            this.imageList1.Images.SetKeyName(301, "SpinningFrame7.png");
            this.imageList1.Images.SetKeyName(302, "StoppedDC_16.png");
            this.imageList1.Images.SetKeyName(303, "subscribe.png");
            this.imageList1.Images.SetKeyName(304, "suse_16x.png");
            this.imageList1.Images.SetKeyName(305, "tools_notinstalled_16x.png");
            this.imageList1.Images.SetKeyName(306, "tools_outofdate_16x.png");
            this.imageList1.Images.SetKeyName(307, "tools_uptodate_16x.png");
            this.imageList1.Images.SetKeyName(308, "tree_minus.png");
            this.imageList1.Images.SetKeyName(309, "tree_plus.png");
            this.imageList1.Images.SetKeyName(310, "tshadowdown.png");
            this.imageList1.Images.SetKeyName(311, "tshadowdownleft.png");
            this.imageList1.Images.SetKeyName(312, "tshadowdownright.png");
            this.imageList1.Images.SetKeyName(313, "tshadowright.png");
            this.imageList1.Images.SetKeyName(314, "tshadowtopright.png");
            this.imageList1.Images.SetKeyName(315, "turbo_16x.png");
            this.imageList1.Images.SetKeyName(316, "ubuntu_16x.png");
            this.imageList1.Images.SetKeyName(317, "upsell_16.png");
            this.imageList1.Images.SetKeyName(318, "usagebar_0.png");
            this.imageList1.Images.SetKeyName(319, "usagebar_1.png");
            this.imageList1.Images.SetKeyName(320, "usagebar_2.png");
            this.imageList1.Images.SetKeyName(321, "usagebar_3.png");
            this.imageList1.Images.SetKeyName(322, "usagebar_4.png");
            this.imageList1.Images.SetKeyName(323, "usagebar_5.png");
            this.imageList1.Images.SetKeyName(324, "usagebar_6.png");
            this.imageList1.Images.SetKeyName(325, "usagebar_7.png");
            this.imageList1.Images.SetKeyName(326, "usagebar_8.png");
            this.imageList1.Images.SetKeyName(327, "usagebar_9.png");
            this.imageList1.Images.SetKeyName(328, "usagebar_10.png");
            this.imageList1.Images.SetKeyName(329, "usb_16.png");
            this.imageList1.Images.SetKeyName(330, "virtualstorage_snapshot_16.png");
            this.imageList1.Images.SetKeyName(331, "vmBackground.png");
            this.imageList1.Images.SetKeyName(332, "vmBackgroundCurrent.png");
            this.imageList1.Images.SetKeyName(333, "VMTemplate_h32bit_32.png");
            this.imageList1.Images.SetKeyName(334, "vnc_local_cursor.png");
            this.imageList1.Images.SetKeyName(335, "windows_h32bit_16.png");
            this.imageList1.Images.SetKeyName(336, "wizard_background.png");
            this.imageList1.Images.SetKeyName(337, "WLB.png");
            this.imageList1.Images.SetKeyName(338, "xcm.png");
            this.imageList1.Images.SetKeyName(339, "xcm_32x32.png");
            this.imageList1.Images.SetKeyName(340, "XS.png");
            this.imageList1.Images.SetKeyName(341, "yinhekylin_16x.png");
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(105, 663);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(240, 217);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            this.richTextBox1.Visible = false;
            // 
            // trvCenter
            // 
            this.trvCenter.BackColor = System.Drawing.SystemColors.ControlLight;
            this.trvCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvCenter.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.trvCenter.ImageKey = "infra_view_16_textured.png";
            this.trvCenter.ImageList = this.imageList1;
            this.trvCenter.ItemHeight = 25;
            this.trvCenter.Location = new System.Drawing.Point(0, 0);
            this.trvCenter.Margin = new System.Windows.Forms.Padding(4);
            this.trvCenter.Name = "trvCenter";
            treeNode2.BackColor = System.Drawing.SystemColors.ControlDark;
            treeNode2.ImageKey = "home_textured.png";
            treeNode2.Name = "Node0";
            treeNode2.Text = "SCADA_CENTER";
            this.trvCenter.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.trvCenter.SelectedImageIndex = 177;
            this.trvCenter.ShowLines = false;
            this.trvCenter.Size = new System.Drawing.Size(213, 560);
            this.trvCenter.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 67);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.richTextBox1);
            this.splitContainer1.Panel1.Controls.Add(this.trvCenter);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1003, 564);
            this.splitContainer1.SplitterDistance = 217;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 9;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.flowLayoutPanel1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tbxRecvData);
            this.splitContainer2.Size = new System.Drawing.Size(777, 560);
            this.splitContainer2.SplitterDistance = 279;
            this.splitContainer2.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(777, 279);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // tbxRecvData
            // 
            this.tbxRecvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxRecvData.Location = new System.Drawing.Point(0, 0);
            this.tbxRecvData.Name = "tbxRecvData";
            this.tbxRecvData.Size = new System.Drawing.Size(777, 277);
            this.tbxRecvData.TabIndex = 1;
            this.tbxRecvData.Text = "";
            // 
            // toolBar
            // 
            this.toolBar.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolBar.Image = ((System.Drawing.Image)(resources.GetObject("toolBar.Image")));
            this.toolBar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(49, 39);
            this.toolBar.Text = "Alarm";
            this.toolBar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(56, 39);
            this.toolStripButton3.Text = "Restart";
            this.toolStripButton3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolBtnStop
            // 
            this.toolBtnStop.Enabled = false;
            this.toolBtnStop.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolBtnStop.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnStop.Image")));
            this.toolBtnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnStop.Name = "toolBtnStop";
            this.toolBtnStop.Size = new System.Drawing.Size(41, 39);
            this.toolBtnStop.Text = "S&top";
            this.toolBtnStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolBtnStop.Click += new System.EventHandler(this.toolBtnStop_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(42, 39);
            this.toolStripButton1.Text = "&Start";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolBtnStop,
            this.toolStripButton4,
            this.toolStripButton3,
            this.toolBar});
            this.toolStrip1.Location = new System.Drawing.Point(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(1003, 42);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(58, 39);
            this.toolStripButton4.Text = "Refresh";
            this.toolStripButton4.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cN51MODEToolStripMenuItem
            // 
            this.cN51MODEToolStripMenuItem.Name = "cN51MODEToolStripMenuItem";
            this.cN51MODEToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.cN51MODEToolStripMenuItem.Text = "CN51 MODE";
            // 
            // openLogFileToolStripMenuItem
            // 
            this.openLogFileToolStripMenuItem.Name = "openLogFileToolStripMenuItem";
            this.openLogFileToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.openLogFileToolStripMenuItem.Text = "Open Log File";
            // 
            // storageToolStripMenuItem
            // 
            this.storageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openLogFileToolStripMenuItem,
            this.cN51MODEToolStripMenuItem});
            this.storageToolStripMenuItem.Name = "storageToolStripMenuItem";
            this.storageToolStripMenuItem.Size = new System.Drawing.Size(43, 19);
            this.storageToolStripMenuItem.Text = "Tool";
            // 
            // vMToolStripMenuItem
            // 
            this.vMToolStripMenuItem.Name = "vMToolStripMenuItem";
            this.vMToolStripMenuItem.Size = new System.Drawing.Size(37, 19);
            this.vMToolStripMenuItem.Text = "VM";
            // 
            // serverToolStripMenuItem
            // 
            this.serverToolStripMenuItem.Name = "serverToolStripMenuItem";
            this.serverToolStripMenuItem.Size = new System.Drawing.Size(51, 19);
            this.serverToolStripMenuItem.Text = "Server";
            // 
            // poolToolStripMenuItem
            // 
            this.poolToolStripMenuItem.Name = "poolToolStripMenuItem";
            this.poolToolStripMenuItem.Size = new System.Drawing.Size(43, 19);
            this.poolToolStripMenuItem.Text = "Pool";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 19);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 19);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.poolToolStripMenuItem,
            this.serverToolStripMenuItem,
            this.vMToolStripMenuItem,
            this.storageToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1003, 25);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SCADACMC.Properties.Resources.SCADA_FOR_CMC502;
            this.pictureBox1.Location = new System.Drawing.Point(551, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(352, 40);
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // tmrCheckMaterialOver10Minds
            // 
            this.tmrCheckMaterialOver10Minds.Interval = 30000;
            this.tmrCheckMaterialOver10Minds.Tick += new System.EventHandler(this.tmrCheckMaterialOver10Minds_Tick);
            // 
            // tmrCheckLossMaterial
            // 
            this.tmrCheckLossMaterial.Interval = 10000;
            // 
            // winsock1
            // 
            this.winsock1.CustomTextEncoding = ((System.Text.Encoding)(resources.GetObject("winsock1.CustomTextEncoding")));
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1003, 631);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblAlertCounter);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ctxTool.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer tmrCheckStatus;
        private System.Windows.Forms.ContextMenuStrip ctxTool;
        private System.Windows.Forms.ToolStripMenuItem itemInspObj;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblAlertCounter;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TreeView trvCenter;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton toolBar;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolBtnStop;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripMenuItem cN51MODEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem storageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem vMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem poolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RichTextBox tbxRecvData;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Timer tmrCheckMaterialOver10Minds;
        private System.Windows.Forms.Timer tmrCheckLossMaterial;
        private Treorisoft.Net.Winsock winsock1;
    }
}

