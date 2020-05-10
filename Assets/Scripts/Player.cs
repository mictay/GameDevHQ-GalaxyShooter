﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private bool _isTripleShotPowerupActive = false;

    [SerializeField]
    private bool _isSpeedPowerupActive = false;

    [SerializeField]
    private bool _isShieldPowerupActive = false;

    [SerializeField]
    private float _speed = 2.5f;

    [SerializeField]
    private float _speedBoost = 4.5f;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private GameObject _speedPrefab;

    [SerializeField]
    private GameObject _shieldPrefab;

    [SerializeField]
    private float _tripleShotOffset = 0f;

    [SerializeField]
    private float _fireRate = 0.5f;

    [SerializeField]
    private float _lastFire = -1.0f;

    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private int _maxLives = 3;

    [SerializeField]
    private float _laserOffset = .8f;

    [SerializeField]
    private int _score;

    private int _deaths = 3;

    private int _tripleShotPowerupTimeToLive = 5;
    private int _speedPowerupTimeToLive = 5;
    private int _shieldPowerupTimeToLive = 5;

    private SpawnManager _spawnManager;

    private GameObject _shieldGraphic;
    private GameObject _thrusterGraphic;
    private GameObject _damageRightEngine;
    private GameObject _damageLeftEngine;
    

    private UIManager _uiManager;

    [SerializeField]
    public AudioSource _laserShotSound;

    [SerializeField]
    public AudioSource _powerUpSound;

    [SerializeField]
    public AudioSource _damageSound;

    [SerializeField]
    public AudioSource _healthSound;

    private int _powerUpTimer = 0;

    /******************************************************************************************
     * Start is called before the first frame update
     */
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        _shieldGraphic = transform.GetChild(0).gameObject;
        _thrusterGraphic = transform.GetChild(1).gameObject;
        _damageRightEngine = transform.GetChild(2).gameObject;
        _damageLeftEngine = transform.GetChild(3).gameObject;

        if (_uiManager == null)
        {
            Debug.LogError("the ui manage is null");
        }

    }

    /******************************************************************************************
     * Update is called once per frame
     */
    void Update()
    {
        CalculateMovement();
        DoFire();
    }

    /******************************************************************************************
     * 
     */
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float speed = _speed;

        if (_isSpeedPowerupActive)
            speed = _speedBoost;

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * speed * Time.deltaTime);

        //bounds for y position
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 3.0f), 0f);

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }

    }

    /******************************************************************************************
     * 
     */
    void DoFire()
    {
        if ((Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0)) && Time.time > _lastFire)
        {
            _lastFire = Time.time + _fireRate;

            if (_isTripleShotPowerupActive)
            {
                Vector3 offset0 = new Vector3(transform.position.x, transform.position.y + _tripleShotOffset, 0);
                GameObject tripleShot = Instantiate(_tripleShotPrefab, offset0, Quaternion.identity);
                tripleShot.transform.GetChild(0).GetComponent<Laser>().setDirectionUp(true);
                tripleShot.transform.GetChild(1).GetComponent<Laser>().setDirectionUp(true);
                tripleShot.transform.GetChild(2).GetComponent<Laser>().setDirectionUp(true);

                _laserShotSound.pitch = 1.0f;
                _laserShotSound.Play();
            }
            //else if (_isSpeedPowerupActive)
            //{
            //    Vector3 offset1 = new Vector3(transform.position.x, transform.position.y + _tripleShotOffset, 0);
            //    Instantiate(_speedPrefab, offset1, Quaternion.identity);
            //}
            //else if (_isShieldPowerupActive)
            //{
            //    Vector3 offset2 = new Vector3(transform.position.x, transform.position.y + _tripleShotOffset, 0);
            //    Instantiate(_shieldPrefab, offset2, Quaternion.identity);
            //}
            else
            {
                Vector3 offset2 = new Vector3(transform.position.x, transform.position.y + _laserOffset, 0);
                Instantiate(_laserPrefab, offset2, Quaternion.identity).GetComponent<Laser>().setDirectionUp(true);
                _laserShotSound.pitch = 2.0f;
                _laserShotSound.Play();
            }

        }

    }

    /******************************************************************************************
     * 
     */
    public void Damage()
    {
        Debug.Log("Damage");

        if (!_isShieldPowerupActive)
        {            

            _lives--;
            _uiManager.SetLives(_lives);

            switch(_lives)
            {
                case 2:
                    _damageSound.Play();
                    _damageRightEngine.SetActive(true);
                    break;

                case 1:
                    _damageSound.Play();
                    _damageLeftEngine.SetActive(true);
                    break;

                case 0:

                    if (_spawnManager != null)
                        _spawnManager.PlayerDied();

                    Destroy(this.gameObject);
                break;
            }


        } else
        {
            //_shieldGraphic.SetActive(false);
            //_isShieldPowerupActive = false;
            Debug.Log("sheild hit: " + _lives);
            _score += 1;
        }

        Debug.Log("lives: " + _lives);

    }

    /******************************************************************************************
     * 
     */
    public void TripleShotPowerUp()
    {
        _isTripleShotPowerupActive = true;
        _powerUpSound.Play();
        StartCoroutine(CoolDownTripleShotPowerUp());
    }

    /******************************************************************************************
     * 
     */
    public void SpeedPowerUp()
    {
        _thrusterGraphic.SetActive(true);
        _isSpeedPowerupActive = true;
        _powerUpSound.Play();
        StartCoroutine(CoolDownSpeedPowerUp());
    }

    /******************************************************************************************
     * 
     */
    public void ShieldPowerUp()
    {
        _shieldGraphic.SetActive(true);
        _isShieldPowerupActive = true;
        _powerUpSound.Play();
        StartCoroutine(CoolDownShieldPowerUp());
    }

    /******************************************************************************************
     * 
     */
    public void HealthPowerUp()
    {
        if (_lives <= _maxLives)
        {
            _healthSound.Play();
            _lives++;
            _uiManager.SetLives(_lives);

            switch (_lives)
            {
                case 2:
                    _damageLeftEngine.SetActive(false);
                    break;

                case 3:
                    _damageRightEngine.SetActive(false);
                    break;
            }

        }

    }

    /******************************************************************************************
     * 
     */
    IEnumerator CoolDownTripleShotPowerUp()
    {
        _powerUpTimer = _speedPowerupTimeToLive;

        while (_powerUpTimer > 0)
        {

            _uiManager.UpdatePowerUpLevel("Triple Shot", _powerUpTimer);
            yield return new WaitForSeconds(1.0f);
            _powerUpTimer--;
        }

        _isTripleShotPowerupActive = false;
        _uiManager.UpdatePowerUpLevel("Triple Shot", 0);
    }

    /******************************************************************************************
     * 
     */
    IEnumerator CoolDownSpeedPowerUp()
    {

        _powerUpTimer = _speedPowerupTimeToLive;

        while (_powerUpTimer > 0)
        {

            _uiManager.UpdatePowerUpLevel("Speed Boost", _powerUpTimer);
            yield return new WaitForSeconds(1.0f);
            _powerUpTimer--;
        }

        _thrusterGraphic.SetActive(false);
        _isSpeedPowerupActive = false;
        _uiManager.UpdatePowerUpLevel("Speed Boost", 0);
    }

    /******************************************************************************************
     * 
     */
    IEnumerator CoolDownShieldPowerUp()
    {
        _powerUpTimer = _shieldPowerupTimeToLive;

        while (_powerUpTimer > 0 )
        {

            _uiManager.UpdatePowerUpLevel("Shields", _powerUpTimer);
            yield return new WaitForSeconds(1.0f);
            _powerUpTimer--;
        }

        _shieldGraphic.SetActive(false);
        _isShieldPowerupActive = false;
        _uiManager.UpdatePowerUpLevel("Shields", 0);
    }

    /******************************************************************************************
     * 
     */
    public void AddScore(int score)
    {
        _score += score;
        _uiManager.UpdateScore(_score);
    }

}